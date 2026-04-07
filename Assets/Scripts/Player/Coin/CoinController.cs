using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CoinController : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The child GameObject holding the coin sprite")]
    public Transform coinVisual;
    [Tooltip("The child GameObject holding the shadow sprite")]
    public Transform coinShadow;

    [Header("Flight Settings")]
    public float arcHeight = 1.5f;
    public float flipSpeed = 8f;

    [Header("Bounce Settings")]
    [Tooltip("How many times the coin bounces before settling")]
    public int bounceCount = 2;
    [Tooltip("Each bounce is this fraction of the previous arc height")]
    [Range(0f, 1f)] public float bounceDamping = 0.45f;
    [Tooltip("Each bounce travels this fraction of the previous distance")]
    [Range(0f, 1f)] public float bounceDistanceDamping = 0.35f;

    [Header("Wall Detection")]
    [Tooltip("LayerMask for walls the coin should collide with mid-flight")]
    public LayerMask wallLayers;
    [Tooltip("How far ahead to raycast each frame (should be >= coin radius)")]
    public float wallCheckDistance = 0.15f;
    [Tooltip("Fraction of arc height kept after a wall ricochet")]
    [Range(0f, 1f)] public float wallArcDamping = 0.6f;
    [Tooltip("Fraction of flight speed kept after a wall ricochet")]
    [Range(0f, 1f)] public float wallSpeedDamping = 0.7f;

    public Action OnLanded;
    public Action OnBounce;
    public Action<Collider2D> OnCollision;

    // ── internal state ──────────────────────────────────────────────
    private Rigidbody2D _rb;
    private CoinVisual   _visual;

    private Vector2 _startPos;
    private Vector2 _targetPos;
    private float   _flightDuration;
    private float   _elapsed;
    private float   _currentArcHeight;

    private int   _bouncesRemaining;
    private bool  _inFlight;
    private bool  _settled;

    // ────────────────────────────────────────────────────────────────
    //  Public Reset — call this right after Instantiate
    // ────────────────────────────────────────────────────────────────
    /// <summary>
    /// Launches the coin from <paramref name="startPos"/> toward
    /// <paramref name="targetPos"/> over <paramref name="duration"/> seconds.
    /// </summary>
    public void ResetCoin(
        Vector2 startPos,
        Vector2 targetPos,
        float   duration      = 0.6f,
        float   arcHeight     = 1.5f,
        int     bounceCount   = 2,
        float   bounceDamping = 0.45f)
    {
        _startPos        = startPos;
        _targetPos       = targetPos;
        _flightDuration  = duration;
        _currentArcHeight = arcHeight;
        _bouncesRemaining = bounceCount;
        _elapsed         = 0f;
        _inFlight        = true;
        _settled         = false;

        // Store configurable values so inspector tweaks still apply
        this.arcHeight     = arcHeight;
        this.bounceCount   = bounceCount;
        this.bounceDamping = bounceDamping;

        transform.position = startPos;

        // Disable Rigidbody2D during scripted flight
        _rb.bodyType = RigidbodyType2D.Kinematic;
        _rb.linearVelocity   = Vector2.zero;
        _rb.angularVelocity = 0f;

        if (_visual != null)
            _visual.BeginFlight(arcHeight, flipSpeed);
    }

    // ────────────────────────────────────────────────────────────────
    //  Unity lifecycle
    // ────────────────────────────────────────────────────────────────
    private void Awake()
    {
        _rb     = GetComponent<Rigidbody2D>();
        _visual = GetComponentInChildren<CoinVisual>();

        // Safety: hide shadow until flight begins
        if (coinShadow != null)
            coinShadow.gameObject.SetActive(false);
    }

    private void Update()
{
    if (!_inFlight) return;

    _elapsed += Time.deltaTime;
    float t = Mathf.Clamp01(_elapsed / _flightDuration);

    // ── Check for walls before moving ──
    Vector2 currentPos  = transform.position;
    Vector2 travelDir   = (_targetPos - currentPos).normalized;
    float   distLeft    = Vector2.Distance(currentPos, _targetPos);

    if (distLeft > 0.01f)
    {
        RaycastHit2D hit = Physics2D.Raycast(
            currentPos, travelDir, 
            Mathf.Min(wallCheckDistance, distLeft), 
            wallLayers);

        if (hit.collider != null)
            HandleWallHit(hit, travelDir);
    }

    // ── Move root along the ground plane ──
    transform.position = Vector2.Lerp(_startPos, _targetPos, t);

    // ── Tell the visual its normalised progress ──
    if (_visual != null)
        _visual.UpdateFlight(t, _currentArcHeight);

    // ── Reached destination ──
    if (t >= 1f)
        HandleLanding();
}

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_settled) return;
        OnCollision?.Invoke(other);
    }

    // ────────────────────────────────────────────────────────────────
    //  Landing & bounce logic
    // ────────────────────────────────────────────────────────────────

    private void HandleWallHit(RaycastHit2D hit, Vector2 travelDir)
    {
        // Reflect travel direction off the wall normal
        Vector2 reflected = Vector2.Reflect(travelDir, hit.normal);

        // How far through the current flight were we?
        float progress = Mathf.Clamp01(_elapsed / _flightDuration);

        // New target: continue in reflected direction for the remaining distance,
        // damped so it doesn't fly as far after a wall hit
        float distRemaining = Vector2.Distance(transform.position, _targetPos);
        _startPos  = transform.position;
        _targetPos = _startPos + reflected * (distRemaining * wallSpeedDamping);

        // Preserve proportional arc height at the moment of impact, then damp it
        _currentArcHeight = _currentArcHeight * (1f - progress) * wallArcDamping;

        // Reset elapsed so the redirected flight plays cleanly from t=0
        // Keep remaining flight duration proportional to remaining distance
        _flightDuration = _flightDuration * (1f - progress) * wallSpeedDamping;
        _elapsed = 0f;

        OnCollision?.Invoke(hit.collider);
    }
    private void HandleLanding()
    {
        _inFlight = false;

        if (_bouncesRemaining > 0)
        {
            StartBounce();
        }
        else
        {
            Settle();
        }
    }

    private void StartBounce()
    {
        _bouncesRemaining--;
        OnBounce?.Invoke();

        // Shrink arc and distance for the next sub-flight
        _currentArcHeight *= bounceDamping;
        float remainingDist = Vector2.Distance(transform.position, _targetPos);
        Vector2 dir = (_targetPos - (Vector2)transform.position).normalized;

        _startPos   = transform.position;
        _targetPos  = _startPos + dir * (remainingDist * bounceDistanceDamping);
        _flightDuration *= bounceDamping;
        _elapsed    = 0f;
        _inFlight   = true;

        if (_visual != null)
            _visual.BeginFlight(_currentArcHeight, flipSpeed * bounceDamping);
    }

    private void Settle()
    {
        _settled = true;
    
        _rb.bodyType = RigidbodyType2D.Dynamic;
        _rb.linearVelocity  = Vector2.zero;
    
        // Switch to trigger so settled coin doesn't block anything
        var col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;
    
        if (_visual != null)
            _visual.Settle();
    
        if (coinShadow != null)
            coinShadow.localScale = Vector3.one;
    
        OnLanded?.Invoke();
    }
}
