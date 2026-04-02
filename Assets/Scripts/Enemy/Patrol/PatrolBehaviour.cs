using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

[RequireComponent(typeof(AIPath))]
[RequireComponent(typeof(GeneralEnemyRotation))]
[RequireComponent(typeof(OceanManager))]
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
    

    void SetState(PatrolState newState)
    {
        if (state == newState) return;
        switch (state)
        {
            case PatrolState.Patrol:
                OnPatrolEnd();
            break;
            case PatrolState.Investigation:
                OnInvestigationEnd();
            break;
            case PatrolState.Return:
                OnReturnEnd();
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
            case PatrolState.Return:
                OnReturnStart();
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
            case PatrolState.Return:
                ReturnUpdate();
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
        // TODO: A* May not be needed on normal patrol behaviour.
        Vector3 target = patrolPoints[currentPatrolTarget].position;
        aStar.destination = target;
    }
    void OnPatrolEnd() {}
    void OnInvestigationStart() {}
    void OnInvestigationEnd() {}
    void OnReturnStart() {}
    void OnReturnEnd() {}
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
                + 0.1*ocean.E;
            if (shouldWait) SetState(PatrolState.Waiting);
            else
            {
                target = patrolPoints[currentPatrolTarget].position;
                aStar.destination = target;
            }
        }
        rotation.LookAt(target);
    }
    void InvestigationUpdate() {}
    void ReturnUpdate() { }

    void WaitingUpdate()
    {
        float lookDrive = (ocean.O + ocean.N) / 2f;
        if (lookDrive < 0.3f) return;

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
    }

    void HandleSoundMade(Vector2 position, float initialLoudness)
    {
        float dst = Vector2.Distance(transform.position, position);
        if (dst > maxHearingDistance) return;

        float perceivedLoudness = initialLoudness / (dst * dst);

        float threshold = 0.5f - (ocean.Neuroticism * 0.4f);
        if (perceivedLoudness < threshold) return;

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
        8.0f);

        yield return new WaitForSeconds(waitTime);

        SetState(PatrolState.Patrol);
    }
}