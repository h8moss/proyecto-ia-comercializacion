using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed;

    private Rigidbody2D rb;
    private PlayerDetection playerDetection;
    private HidingBehaviour hb;
    public Vector2 Movement { get; private set; }

    private int GeneralMultiplier = 1;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerDetection = GetComponent<PlayerDetection>();
        hb = GetComponent<HidingBehaviour>();

        playerDetection.OnDeath += OnDeactivate;
        hb.OnHidden += OnDeactivate;
        hb.OnUnhidden += OnReactivate;
    }

    void OnDestroy()
    {
        playerDetection.OnDeath -= OnDeactivate;
        hb.OnHidden -= OnDeactivate;
        hb.OnUnhidden -= OnReactivate;
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
        rb.linearVelocity = speed * Time.fixedDeltaTime * Movement * GeneralMultiplier;
    }

    void OnDeactivate()
    {
        GeneralMultiplier = 0;
    }
    void OnReactivate()
    {
        GeneralMultiplier = 1;
    }
}
