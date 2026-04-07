using UnityEngine;

/// <summary>
/// Handles the fake-arc height offset, coin flip, spin, and shadow scaling.
/// Sits on the child visual object so the parent's world position stays
/// on the ground plane at all times.
/// </summary>
public class CoinVisual : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Drag the Shadow child transform here")]
    public Transform shadow;

    [Header("Spin")]
    [Tooltip("Degrees per second the coin spins on its Z axis")]
    public float spinSpeed = 360f;

    // ── internal ──
    private float _arcHeight;
    private float _flipSpeed;
    private bool  _flying;
    private bool  _settling;
    private float _settleTimer;
    private float _spinVelocity;

    // ────────────────────────────────────────────────────────────────

    public void BeginFlight(float arcHeight, float flipSpeed)
    {
        _arcHeight  = arcHeight;
        _flipSpeed  = flipSpeed;
        _flying     = true;
        _settling   = false;
        _spinVelocity = spinSpeed;

        if (shadow != null)
            shadow.gameObject.SetActive(true);
    }

    public void UpdateFlight(float normalizedTime, float arcHeight)
    {
        _arcHeight = arcHeight;

        // ── Parabolic height offset ──
        float height = _arcHeight * Mathf.Sin(Mathf.PI * normalizedTime);
        transform.localPosition = new Vector3(0f, height, 0f);

        // ── Coin-flip illusion (squish X axis) ──
        float flip = Mathf.Abs(Mathf.Sin(_flipSpeed * normalizedTime * Mathf.PI));
        transform.localScale = new Vector3(flip, 1f, 1f);

        // ── Shadow: shrinks as coin rises, shifts slightly ──
        if (shadow != null)
        {
            float shadowScale = Mathf.Lerp(1f, 0.4f, height / Mathf.Max(_arcHeight, 0.01f));
            shadow.localScale = new Vector3(shadowScale, shadowScale, 1f);
        }
    }

    public void Settle()
    {
        _flying   = false;
        _settling = true;
        _settleTimer = 0f;
        transform.localPosition = Vector3.zero;
    }

    private void Update()
    {
        if (!_flying && !_settling) return;

        // ── Continuous Z spin ──
        transform.Rotate(0f, 0f, _spinVelocity * Time.deltaTime);

        if (_settling)
        {
            // Slow the spin to a stop over ~0.6 s
            _settleTimer += Time.deltaTime;
            _spinVelocity = Mathf.Lerp(spinSpeed, 0f, _settleTimer / 0.6f);

            // Snap scale back to full circle
            transform.localScale = Vector3.Lerp(
                transform.localScale, Vector3.one, Time.deltaTime * 10f);

            if (_settleTimer >= 0.6f)
            {
                _settling     = false;
                _spinVelocity = 0f;
                transform.localScale    = Vector3.one;
                transform.localPosition = Vector3.zero;

                if (shadow != null)
                    shadow.localScale = Vector3.one;
            }
        }
    }
}
