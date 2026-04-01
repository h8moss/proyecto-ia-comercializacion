using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GeneralEnemyMovement))]
[RequireComponent(typeof(GeneralEnemyRotation))]
[RequireComponent(typeof(OceanManager))]
public class PatrolBehaviour : MonoBehaviour
{

    [SerializeField] private List<Transform> patrolPoints;
    [SerializeField] private float maxHearingDistance;
    private int currentPatrolTarget;
    private PatrolState state;
    private GeneralEnemyMovement movement;
    private GeneralEnemyRotation rotation;
    private OceanManager ocean;

    private PatrolState State
    {
        set
        {
            state = value;

            if (state == PatrolState.Patrol) {
                Vector2 target = (Vector2)patrolPoints[currentPatrolTarget].position;
                movement.SetPath(new List<Vector2>{transform.position, target});
            }
        }
    }

    void Start()
    {
        ocean = GetComponent<OceanManager>();
        movement = GetComponent<GeneralEnemyMovement>();
        rotation = GetComponent<GeneralEnemyRotation>();
        movement.OnFinishedPath += OnReachedPoint;
        State = PatrolState.Patrol;
        currentPatrolTarget = 0;
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
        }
    }

    void PatrolUpdate()
    {
    }

    void InvestigationUpdate()
    {
    }

    void ReturnUpdate()
    {
    }

    void OnReachedPoint()
    {
        if (state == PatrolState.Patrol)
        {
            currentPatrolTarget++;
            currentPatrolTarget %= patrolPoints.Count;

            Vector2 target = (Vector2)patrolPoints[currentPatrolTarget].position;
            movement.SetPath(new List<Vector2>{transform.position, target});
            rotation.LookAt(target);
        }
    }

    void HandleSoundMade(Vector2 position, float initialLoudness)
    {
        float dst = Vector2.Distance(transform.position, position);
        if (dst > maxHearingDistance) return;

        float perceivedLoudness = initialLoudness / (dst * dst);

        float threshold = 0.5f - (ocean.Neuroticism * 0.4f);
        if (perceivedLoudness < threshold) return;

        // Look at sound + walk to it?
    }
}