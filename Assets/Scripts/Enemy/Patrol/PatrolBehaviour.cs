using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GeneralEnemyMovement))]
[RequireComponent(typeof(GeneralEnemyRotation))]
[RequireComponent(typeof(OceanManager))]
public class PatrolBehaviour : MonoBehaviour
{
    [Header("Patrol")]
    [SerializeField] private List<Transform> patrolPoints;
    [SerializeField] private float basePatrolSpeed = 2f;

    [Header("Hearing")]
    [SerializeField] private float maxHearingDistance = 10f;
    [SerializeField] private float baseInvestigationDuration = 4f;  // How long to linger at sound source
    [SerializeField] private float baseLoudnessThreshold = 0.5f;

    private int currentPatrolTarget;
    private PatrolState state;
    private GeneralEnemyMovement movement;
    private GeneralEnemyRotation rotation;
    private OceanManager ocean;

    // Investigation state
    private Vector2 investigationTarget;
    private float investigationTimer;
    private Vector2 returnTarget; // Where to return after investigating

    // ── Derived OCEAN values (computed once in Start) ──────────────────────
    private float patrolSpeed;           // E↑ = faster patrol
    private float loudnessThreshold;     // N↑ = lower threshold (more reactive)
    private float investigationDuration; // O↑ = lingers longer; C↑ = shorter
    private float investigationSpeed;    // E↑ = rushes toward sound
    private float giveUpChance;          // A↑ = more likely to abandon mid-investigation

    private PatrolState State
    {
        set
        {
            state = value;

            switch (state)
            {
                case PatrolState.Patrol:
                    // movement.SetSpeed(patrolSpeed);
                    Vector2 target = (Vector2)patrolPoints[currentPatrolTarget].position;
                    movement.SetPath(new List<Vector2> { transform.position, target });
                    break;

                case PatrolState.Investigation:
                    // E drives how urgently the enemy approaches the sound
                    // movement.SetSpeed(investigationSpeed);
                    movement.SetPath(new List<Vector2> { transform.position, investigationTarget });
                    rotation.LookAt(investigationTarget);
                    investigationTimer = investigationDuration;
                    break;

                case PatrolState.Return:
                    // movement.SetSpeed(patrolSpeed);
                    returnTarget = (Vector2)patrolPoints[currentPatrolTarget].position;
                    movement.SetPath(new List<Vector2> { transform.position, returnTarget });
                    rotation.LookAt(returnTarget);
                    break;
            }
        }
    }

    void Start()
    {
        ocean = GetComponent<OceanManager>();
        movement = GetComponent<GeneralEnemyMovement>();
        rotation = GetComponent<GeneralEnemyRotation>();

        movement.OnFinishedPath += OnReachedPoint;
        WorldEvents.OnSoundMade += HandleSoundMade;   // ← subscribe here

        ComputeOceanStats();
        currentPatrolTarget = 0;
        State = PatrolState.Patrol;
    }

    void OnDestroy()
    {
        movement.OnFinishedPath -= OnReachedPoint;
        WorldEvents.OnSoundMade -= HandleSoundMade;   // ← always unsubscribe
    }


    /// <summary>
    /// Translates raw OCEAN [0..1] values into concrete behaviour parameters.
    /// Call this once at Start, or again if traits change at runtime.
    /// </summary>
    void ComputeOceanStats()
    {
        // Neuroticism: jumpier enemies react to quieter sounds
        // N=0 → threshold = base + 0.4 (hard to startle)
        // N=1 → threshold = base - 0.4 (startles at a whisper)
        loudnessThreshold = baseLoudnessThreshold - (ocean.Neuroticism * 0.4f);

        // Extraversion: drives speed — bold enemies patrol and rush faster
        // Range: 70%–130% of base speed
        float extraversionSpeedMult = 0.7f + (ocean.Extraversion * 0.6f);
        patrolSpeed = basePatrolSpeed * extraversionSpeedMult;
        investigationSpeed = patrolSpeed * 1.4f; // always faster than patrol

        // Openness: curious enemies linger longer at investigation points
        // Conscientiousness: disciplined enemies wrap up investigations quickly
        // Net effect: O pulls up, C pulls down
        float curiosityFactor = 0.5f + (ocean.Openness * 0.8f);       // 0.5–1.3×
        float disciplineFactor = 1.5f - (ocean.Conscientiousness * 0.8f); // 0.7–1.5×
        investigationDuration = baseInvestigationDuration * curiosityFactor * disciplineFactor;

        // Agreeableness: agreeable (non-aggressive) enemies are more likely
        // to second-guess themselves and abandon an investigation mid-way
        // A=0 → 0% chance to give up each second
        // A=1 → 15% chance to give up each second
        giveUpChance = ocean.Agreeableness * 0.15f;
    }

    void Update()
    {
        switch (state)
        {
            case PatrolState.Patrol:       PatrolUpdate();       break;
            case PatrolState.Investigation: InvestigationUpdate(); break;
            case PatrolState.Return:       ReturnUpdate();       break;
        }
    }

    void PatrolUpdate()
    {
        // Nothing extra needed — movement handles pathing,
        // and OnReachedPoint advances the waypoint loop.
    }

    void InvestigationUpdate()
    {
        investigationTimer -= Time.deltaTime;

        // Agreeableness: probabilistic early give-up (checked per second)
        // This represents the enemy talking themselves out of it
        if (giveUpChance > 0f && Random.value < giveUpChance * Time.deltaTime)
        {
            State = PatrolState.Return;
            return;
        }

        if (investigationTimer <= 0f)
        {
            // Conscientiousness: high-C enemies return to *exact* patrol point.
            // Low-C enemies are sloppier and just resume from wherever they are.
            if (ocean.Conscientiousness > 0.5f)
            {
                State = PatrolState.Return;
            }
            else
            {
                // Sloppy — just resume patrol from current position
                State = PatrolState.Patrol;
            }
        }
    }

    void ReturnUpdate()
    {
        // Movement will call OnReachedPoint when we arrive,
        // which will flip us back to Patrol.
    }

    void OnReachedPoint()
    {
        switch (state)
        {
            case PatrolState.Patrol:
                currentPatrolTarget = (currentPatrolTarget + 1) % patrolPoints.Count;
                Vector2 next = (Vector2)patrolPoints[currentPatrolTarget].position;
                movement.SetPath(new List<Vector2> { transform.position, next });
                rotation.LookAt(next);
                break;

            case PatrolState.Investigation:
                // Arrived at sound source — start the lingering countdown
                // (timer is already running from when state was set)
                rotation.LookAt(investigationTarget);
                break;

            case PatrolState.Return:
                State = PatrolState.Patrol;
                break;
        }
    }

    void HandleSoundMade(Vector2 position, float initialLoudness)
    {
        float dst = Vector2.Distance(transform.position, position);
        if (dst > maxHearingDistance) return;

        float perceivedLoudness = initialLoudness / (dst * dst);
        if (perceivedLoudness < loudnessThreshold) return;

        // Openness: open enemies investigate sounds even mid-investigation
        // (they're curious about the *new* sound, not anchored to the old one).
        // Closed enemies (O < 0.4) ignore new sounds while already investigating.
        bool isAlreadyInvestigating = state == PatrolState.Investigation;
        if (isAlreadyInvestigating && ocean.Openness < 0.4f) return;

        investigationTarget = position;
        State = PatrolState.Investigation;
    }
}