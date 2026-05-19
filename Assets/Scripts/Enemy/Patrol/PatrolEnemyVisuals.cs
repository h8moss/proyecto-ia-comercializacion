using Pathfinding;
using UnityEngine;

[RequireComponent(typeof(AIPath))]
public class PatrolEnemyVisuals : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private bool isMoving;

    private AIPath path;

    void Start()
    {
        path = GetComponent<AIPath>();
    }

    void Update()
    {
        Vector2 movement = path.velocity;
        if (movement.magnitude > 1f)
        {
            if (!isMoving)
            {
                animator.SetBool("IsMoving", true);
            }
        } else
        {
            if (isMoving)
            {
                animator.SetBool("IsMoving", false);
            }
        }
        isMoving = movement.magnitude > 1f;
    }
}
