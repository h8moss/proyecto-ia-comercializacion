using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;


[RequireComponent(typeof(GeneralEnemyRotation))]
[RequireComponent(typeof(DetectionArc))]
public class BbegBehaviour : MonoBehaviour
{
    [SerializeField] private List<Transform> patrolPoints;
    [SerializeField] private float thinkingTime;
    [SerializeField] private float playerStaticTime;
    [SerializeField] private float cellSize = 10f; // world units
    [SerializeField] private float seekingSpeedMultiplier = 1.2f;
    [SerializeField] private float seekingTurnSpeedMultiplier = 1.2f;
    [SerializeField] private float hearingThreshold = 3.0f;
    [SerializeField] private float talkingCooldown = 5.0f;
    [SerializeField] private GameObject talkingObj;

    [SerializeField] private AIPath aStar;


    private Vector2Int currentCell;
    private float timeInCell = 0f;
    private BbegState state;
    private int currentPatrolTarget;
    private GeneralEnemyRotation rotation;
    private float initialSpeed;
    private float initialRotationSpeed;
    private Vector3 seekingTarget;
    private float currentTalkingCooldown;
    
    void Start()
    {
        // WorldEvents.OnSoundMade += HandleSoundMade;
        currentPatrolTarget = 0;
        rotation = GetComponent<GeneralEnemyRotation>();
        initialSpeed = aStar.maxSpeed;
        initialRotationSpeed = rotation.rotationSpeed;
        currentTalkingCooldown = talkingCooldown;

        WorldEvents.OnSoundMade += HandleSoundMade;
        GetComponent<DetectionArc>().OnPlayerDetected += OnPlayerDetected;

        // Start with patrol
        SetState(BbegState.Patrol);
        OnPatrolStart();
    }

    void OnDestroy()
    {
        WorldEvents.OnSoundMade -= HandleSoundMade;
        GetComponent<DetectionArc>().OnPlayerDetected -= OnPlayerDetected;
    }

    void SetState(BbegState newState)
    {
        switch (state)
        {
            case BbegState.Patrol:
                OnPatrolEnd();
            break;
            case BbegState.Thinking:
                OnThinkingEnd();
            break;
            case BbegState.Talking:
                OnTalkingEnd();
            break;
            case BbegState.Seeking:
                OnSeekingEnd();
            break;
            default:
                Debug.LogError("Missing exit callback for patrol state " + state);
            break;
        }
        state = newState;
        switch (state)
        {
            case BbegState.Patrol:
                OnPatrolStart();
            break;
            case BbegState.Thinking:
                OnThinkingStart();
            break;
            case BbegState.Talking:
                OnTalkingStart();
            break;
            case BbegState.Seeking:
                OnSeekingStart();
            break;
            default:
                Debug.LogError("Missing start callback for patrol state " + state);
            break;
        }
    }

    void Update()
    {
        Vector3 playerPos = PlayerLocator.Player.position;

        Vector2Int newCell = new(
            Mathf.FloorToInt(playerPos.x / cellSize),
            Mathf.FloorToInt(playerPos.z / cellSize)
        );

        if (newCell == currentCell) {
            timeInCell += Time.deltaTime;
        } else {
            currentCell = newCell;
            timeInCell = 0f;
        }

        switch (state)
        {
            case BbegState.Patrol:
                PatrolUpdate();
                break;
            case BbegState.Seeking:
                SeekingUpdate();
                break;
            case BbegState.Talking:
                TalkingUpdate();
                break;
            case BbegState.Thinking:
                ThinkingUpdate();
                break;
        }
    }

    void OnPatrolStart()
    {
        Vector3 target = patrolPoints[currentPatrolTarget].position;
        aStar.destination = target;
    }
    void OnThinkingStart()
    {
        aStar.destination = transform.position;
        StartCoroutine(ThinkingRoutine());
    }
    void OnTalkingStart()
    {
        aStar.destination = transform.position;
        talkingObj.SetActive(true);
    }
    void OnSeekingStart()
    {
        aStar.maxSpeed = initialSpeed * seekingSpeedMultiplier;
        rotation.rotationSpeed = initialRotationSpeed * seekingTurnSpeedMultiplier;
    

        aStar.destination = seekingTarget;
    }

    void OnPatrolEnd() {}
    void OnThinkingEnd() {}
    void OnTalkingEnd()
    {
        talkingObj.SetActive(false);
    }
    void OnSeekingEnd()
    {
        aStar.maxSpeed = initialSpeed;
        rotation.rotationSpeed = initialRotationSpeed;
    }

    void PatrolUpdate() {
        Vector3 target = patrolPoints[currentPatrolTarget].position;
        float dst = Vector3.Distance(target, transform.position);
        if (dst < 1.0f)
        {
            currentPatrolTarget++;
            currentPatrolTarget %= patrolPoints.Count;

            bool shouldWait = Random.value < 0.7;

            if (shouldWait) SetState(BbegState.Thinking);
            else
            {
                target = patrolPoints[currentPatrolTarget].position;
                aStar.destination = target;
            }
        }
        CheckScare();
        rotation.LookAt(target);
    }
    void SeekingUpdate()
    {
        float dst = Vector3.Distance(aStar.destination, transform.position);
        if (dst < 1.0f)
        {
            SetState(BbegState.Thinking);
        }

        rotation.LookAt(seekingTarget);
    }
    void TalkingUpdate() {}
    void ThinkingUpdate() {}

    IEnumerator ThinkingRoutine()
    {
        bool shouldSeek = timeInCell >= playerStaticTime;
        float waitTime = 2 + Random.value+2; // 2 to 4 seconds
        yield return new WaitForSeconds(waitTime);

        if (shouldSeek) seekingTarget = PlayerLocator.Player.position;
        SetState(shouldSeek ? BbegState.Seeking : BbegState.Patrol);
    }

    void HandleSoundMade(Vector2 position, float initialLoudness)
    {
        float dst = Vector2.Distance(transform.position, position);

        float perceivedLoudness = initialLoudness / (dst * dst);

        if (perceivedLoudness < hearingThreshold) return;

        Debug.Log("BBEG Heard you!");

        seekingTarget = position;
        SetState(BbegState.Seeking);
    }

    void OnPlayerDetected(Vector3 playerPos)
    {
        seekingTarget = playerPos;
        SetState(BbegState.Seeking);
    }

    void CheckScare()
    {
        currentTalkingCooldown -= Time.deltaTime;
        if (currentTalkingCooldown > 0.0f) return;

        currentTalkingCooldown = talkingCooldown;
        var collisions = Physics2D.CircleCastAll(transform.position, 4.0f, Vector2.up);
        List<ScareableEnemy> scareds = new();

        foreach (var col in collisions)
        {
            if (col.collider.gameObject.TryGetComponent<ScareableEnemy>(out var scared))
            {
                if (!scared.IsScared)
                    scareds.Add(scared);
            }
        }

        Debug.Log(scareds.Count);
        if (scareds.Count > 0 && Random.value < 0.5)
        {
            StartCoroutine(ScareCoroutine(scareds));
        }
    }

    IEnumerator ScareCoroutine(List<ScareableEnemy> scareds)
    {
        SetState(BbegState.Talking);
        yield return new WaitForSeconds(1+Random.value*2);
        foreach (var scared in scareds)
        {
            scared.Scare();
        }
        SetState(BbegState.Patrol);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, 4.0f);
    }
}
