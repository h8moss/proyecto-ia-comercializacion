using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

[RequireComponent(typeof(AIPath))]
[RequireComponent(typeof(GeneralEnemyRotation))]
[RequireComponent(typeof(OceanManager))]
[RequireComponent(typeof(ScareableEnemy))]
public class PatrolBehaviour : MonoBehaviour
{

    [SerializeField] private List<Transform> patrolPoints;
    [SerializeField] private float maxHearingDistance;
    [SerializeField] private float waitingLookInterval = 4f;
    [SerializeField] private float waitingLookAngleRange = 90f;

    private int currentPatrolTarget;
    private PatrolState state;
    private AIPath aStar;
    private GeneralEnemyRotation rotation;
    private OceanManager ocean;
    private float waitingLookTimer = 0f;
    private Quaternion waitingInitialRotation;
    private Vector3 investigationTarget;
    private ScareableEnemy scare;
    

    void SetState(PatrolState newState)
    {
        switch (state)
        {
            case PatrolState.Patrol:
                OnPatrolEnd();
            break;
            case PatrolState.Investigation:
                OnInvestigationEnd();
            break;
            case PatrolState.Waiting:
                OnWaitingEnd();
            break;
            default:
                Debug.LogError("Missing exit callback for patrol state");
            break;
        }
        state = newState;
        switch (state)
        {
            case PatrolState.Patrol:
                OnPatrolStart();
            break;
            case PatrolState.Investigation:
                OnInvestigationStart();
            break;
            case PatrolState.Waiting:
                OnWaitingStart();
            break;
            default:
                Debug.LogError("Missing start callback for patrol state");
            break;
        }
    }

    void Start()
    {
        WorldEvents.OnSoundMade += HandleSoundMade;
        currentPatrolTarget = 0;
        aStar = GetComponent<AIPath>();
        rotation = GetComponent<GeneralEnemyRotation>();
        ocean = GetComponent<OceanManager>();
        scare = GetComponent<ScareableEnemy>();
        scare.ScaredChanged += OnScaredChanged;

        // Start with patrol
        SetState(PatrolState.Patrol);
        OnPatrolStart();
    }

    void Update()
    {
        switch (state)
        {
            case PatrolState.Patrol:
                PatrolUpdate();
            break;
            case PatrolState.Investigation:
                InvestigationUpdate();
            break;
            case PatrolState.Waiting:
                WaitingUpdate();
            break;
            default:
                Debug.LogError("Missing update implementation for patrol state");
            break;
        }
    }

    void OnPatrolStart()
    {
        Debug.Log("Patrol");
        // TODO: A* May not be needed on normal patrol behaviour.
            Vector3 target = patrolPoints[currentPatrolTarget].position;
        aStar.destination = target;
    }
    void OnPatrolEnd() {}
    void OnInvestigationStart()
    {
        Debug.Log("Going towards: " + investigationTarget);
        aStar.destination = investigationTarget;
    }
    void OnInvestigationEnd()
    {
    }
    void OnWaitingStart()
    {
        waitingInitialRotation = transform.rotation;
        aStar.destination = transform.position;
        StartCoroutine(WaitingRoutine());
    }
    void OnWaitingEnd() {}
    void PatrolUpdate()
    {
        Vector3 target = patrolPoints[currentPatrolTarget].position;
        float dst = Vector3.Distance(target, transform.position);
        if (dst < 1.0f)
        {
            currentPatrolTarget++;
            currentPatrolTarget %= patrolPoints.Count;

            bool shouldWait = Random.value < 
                0.05 
                + 0.4*(1-ocean.C) 
                + 0.25*ocean.N 
                + 0.2*ocean.O 
                + 0.1*ocean.E
                + 0.4*scare.Multiplier;
            if (shouldWait) SetState(PatrolState.Waiting);
            else
            {
                target = patrolPoints[currentPatrolTarget].position;
                aStar.destination = target;
            }
        }
        rotation.LookAt(target);
    }
    void InvestigationUpdate()
    {
        float dst = Vector3.Distance(transform.position, investigationTarget);
        if (dst < 1.0f)
        {
            // If we switch patrol to not use A*, this won't work nomore.
            // We'll have to implement a new state: Return
            SetState(PatrolState.Patrol);
        }
        rotation.LookAt(investigationTarget);
    }

    void WaitingUpdate()
    {
        float lookDrive = (ocean.O + ocean.N) / 2f;
        if (lookDrive < 0.3f && !scare.IsScared) return;

        waitingLookTimer -= Time.deltaTime;
        if (waitingLookTimer <= 0f)
        {
            if (Random.value < lookDrive)
                rotation.LookAtRandomAngle(waitingInitialRotation, waitingLookAngleRange);

            waitingLookTimer = waitingLookInterval * (1f - lookDrive * 0.6f);
        }   
    }

    void OnDestroy()
    {
        WorldEvents.OnSoundMade -= HandleSoundMade;
        scare.ScaredChanged -= OnScaredChanged;
    }

    void HandleSoundMade(Vector2 position, float initialLoudness)
    {
        float dst = Vector2.Distance(transform.position, position);
        if (dst > maxHearingDistance) return;

        float perceivedLoudness = initialLoudness / (dst * dst);

        float n = ocean.Neuroticism;
        float threshold = Mathf.Lerp(8f, 0.3f, n * n * n); // Much less aggressive to sounds
        if (perceivedLoudness < threshold) return;
        if (Random.value >= ocean.Openness) return;

        Debug.Log("Heard you!");

        investigationTarget = position;
        SetState(PatrolState.Investigation);
    }

    IEnumerator WaitingRoutine()
    {
        float waitTime = Mathf.Clamp(
            (1.5f
            + 2.0f*ocean.N
            + 1.5f*ocean.O
            + 2.5f*(1-ocean.C)
            + 1.0f*(1-ocean.E))
            * Random.Range(0.75f, 1.25f),
        0.5f,
        8.0f - scare.Multiplier * 4);

        Debug.Log("Waiting: " + waitTime);

        yield return new WaitForSeconds(waitTime);

        SetState(PatrolState.Patrol);
    }

    private void OnScaredChanged(bool scared)
    {
        float delta = aStar.maxSpeed / 3.0f;
        if (scared) aStar.maxSpeed += delta;
        else aStar.maxSpeed -= delta;
    }
}