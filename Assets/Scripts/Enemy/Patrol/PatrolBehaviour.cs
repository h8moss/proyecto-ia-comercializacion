using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GeneralEnemyMovement))]
public class PatrolBehaviour : MonoBehaviour
{

    [SerializeField] private List<Transform> patrolPoints;
    private int currentPatrolTarget;
    private PatrolState state;
    private GeneralEnemyMovement movement;

    void Start()
    {
        movement = GetComponent<GeneralEnemyMovement>();
        state = PatrolState.Patrol;
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
        Vector2 target = (Vector2)patrolPoints[currentPatrolTarget].position;
        movement.SetPath(new List<Vector2>{transform.position, target});

        
        if (Vector2.Distance(transform.position, target) < 0.1f)
        {
            currentPatrolTarget++;
            currentPatrolTarget %= patrolPoints.Count;
        }
    }

    void InvestigationUpdate()
    {

    }

    void ReturnUpdate()
    {

    }
}
