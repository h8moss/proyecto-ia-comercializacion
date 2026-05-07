using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

[RequireComponent(typeof(AIPath))]
[RequireComponent(typeof(GeneralEnemyRotation))]
public class BbegBehaviour : MonoBehaviour
{
    [SerializeField] private List<Transform> patrolPoints;
    [SerializeField] private float thinkingTime;
    [SerializeField] private float playerStaticTime;
    [SerializeField] private float cellSize = 10f; // world units
    [SerializeField] private int gridRadius = 5; // how many cells around the player to draw


    private Vector2Int currentCell;
    private float timeInCell = 0f;
    private BbegState state;
    private int currentPatrolTarget;
    private AIPath aStar;
    private GeneralEnemyRotation rotation;
    private Quaternion thinkingInitialRotation;
    
    void Start()
    {
        // WorldEvents.OnSoundMade += HandleSoundMade;
        currentPatrolTarget = 0;
        aStar = GetComponent<AIPath>();
        rotation = GetComponent<GeneralEnemyRotation>();

        // Start with patrol
        SetState(BbegState.Patrol);
        OnPatrolStart();
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

        Vector2Int newCell = new Vector2Int(
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

    void OnPatrolStart() {}
    void OnThinkingStart()
    {
        thinkingInitialRotation = transform.rotation;
        aStar.destination = transform.position;
        StartCoroutine(ThinkingRoutine());
    }
    void OnTalkingStart() {}
    void OnSeekingStart() {}

    void OnPatrolEnd() {}
    void OnThinkingEnd() {}
    void OnTalkingEnd() {}
    void OnSeekingEnd() {}

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
        rotation.LookAt(target);
    }
    void SeekingUpdate() {}
    void TalkingUpdate() {}
    void ThinkingUpdate() {}

    IEnumerator ThinkingRoutine()
    {
        bool shouldSeek = timeInCell >= playerStaticTime;
        float waitTime = 2 + Random.value+2; // 2 to 4 seconds
        yield return new WaitForSeconds(waitTime);

        SetState(shouldSeek ? BbegState.Seeking : BbegState.Patrol);
    }

    private void OnDrawGizmosSelected()
    {
        Transform player = PlayerLocator.Player;
        Gizmos.color = Color.gray;

        Vector3 playerPos = player.position;

        int centerX = Mathf.FloorToInt(playerPos.x / cellSize);
        int centerZ = Mathf.FloorToInt(playerPos.z / cellSize);

        float size = gridRadius * cellSize;

        // Draw vertical lines
        for (int x = -gridRadius; x <= gridRadius; x++)
        {
            float worldX = (centerX + x) * cellSize;

            Vector3 start = new Vector3(worldX, playerPos.y, (centerZ * cellSize) - size);
            Vector3 end   = new Vector3(worldX, playerPos.y, (centerZ * cellSize) + size);

            Gizmos.DrawLine(start, end);
        }

        // Draw horizontal lines
        for (int z = -gridRadius; z <= gridRadius; z++)
        {
            float worldZ = (centerZ + z) * cellSize;

            Vector3 start = new Vector3((centerX * cellSize) - size, playerPos.y, worldZ);
            Vector3 end   = new Vector3((centerX * cellSize) + size, playerPos.y, worldZ);

            Gizmos.DrawLine(start, end);
        }

        // Highlight current cell
        Gizmos.color = Color.red;

        Vector3 cellCenter = new Vector3(
            (currentCell.x * cellSize) + cellSize * 0.5f,
            playerPos.y,
            (currentCell.y * cellSize) + cellSize * 0.5f
        );

        Gizmos.DrawWireCube(
            cellCenter,
            new Vector3(cellSize, 0.1f, cellSize)
        );
    }
}
