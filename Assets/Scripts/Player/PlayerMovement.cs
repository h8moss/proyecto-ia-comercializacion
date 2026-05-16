using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed;

    private Rigidbody2D rb;
    private PlayerDetection playerDetection;
    public Vector2 Movement { get; private set; }

    private int DeathMultiplier = 1;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerDetection = GetComponent<PlayerDetection>();
        playerDetection.OnDeath += OnDeath;
    }

    void OnDestroy()
    {
        playerDetection.OnDeath -= OnDeath;
    }

    void Update()
    {
        Movement = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        ).normalized;
    }

    void FixedUpdate()
    {
        rb.linearVelocity = speed * Time.fixedDeltaTime * Movement * DeathMultiplier;
    }

    void OnDeath()
    {
        DeathMultiplier = 0;
    }
}
