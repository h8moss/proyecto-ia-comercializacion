using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerDetection))]
public class PlayerVisual : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Transform sprite;
    [SerializeField] private bool allowDiagonal;


    private bool isMoving;
    private bool isDead;

    private PlayerMovement playerMovement;
    private PlayerDetection playerDetection;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerDetection = GetComponent<PlayerDetection>();
        playerDetection.healthChanged += HealthChanged;
    }

    void OnDestroy()
    {
        playerDetection.healthChanged -= HealthChanged;
    }

    void Update()
    {
        if (isDead) return;
        Vector2 movement = playerMovement.Movement;
        if (movement.magnitude > 0.1f)
        {
            if (!isMoving)
            {
                animator.SetBool("isMoving", true);
            }

            RotatePlayer(movement);
        } else
        {
            if (isMoving)
            {
                animator.SetBool("isMoving", false);
            }
        }
        isMoving = movement.magnitude > 0.1f;
    }

    void RotatePlayer(Vector2 movement)
    {
        float x = movement.x;
        float y = movement.y;
        float aX = Mathf.Abs(movement.x);
        float aY = Mathf.Abs(movement.y);
        float nX = - movement.x;
        float nY = - movement.y;
        float e = 0.1f;

        float angle = 0;

        if (!allowDiagonal && y > e || allowDiagonal && y > e && aX < e)
        {
            angle = 0;
        } else if (allowDiagonal && y > e && x > e)
        {
            angle = -45;
        } else if (!allowDiagonal && x > e || allowDiagonal && x > e && aY < e)
        {
            angle = -90;
        } else if (allowDiagonal && x > e && nY > e)
        {
            angle = -135;
        } else if (!allowDiagonal && nY > e || allowDiagonal && nY > e && aX < e)
        {
            angle = -180;
        } else if (allowDiagonal && nY > e && nX > e)
        {
            angle = -225;
        } else if (!allowDiagonal && nX > e || allowDiagonal && nX > e && aY < e)
        {
            angle = -270;
        } else if (allowDiagonal && y > e && nX > e)
        {
            angle = -315;
        }

        sprite.rotation = Quaternion.Euler(0,0,angle);
    }

    void HealthChanged(float h)
    {
        if (isDead) return;
        if (h <= 0)
        {
            isMoving = false;
            isDead = true;
            animator.SetTrigger("Death");
        }
    }
}
