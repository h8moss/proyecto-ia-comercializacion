using System;
using UnityEngine;

public class DetectionArc : MonoBehaviour
{
    [SerializeField] private float length;
    [SerializeField] private float angle;
    [SerializeField] private LayerMask visionObstacles;

    public float Length { get => length; set => length = value; }
    public float Angle { get => angle; set => angle = value; }
    public LayerMask VisionObstacles { get => visionObstacles; }

    public Action<Vector3> OnPlayerDetected;
    public Action<Vector3> OnPlayerHidden;

    public void SetDetectionField(float length, float angle)
    {
        this.length = length;
        this.angle = angle;
    }

    private bool playerInVision = false;

    void Update()
    {
        bool detected = DetectPlayer();

        if (detected && !playerInVision)
        {
            playerInVision = true;
            DetectionEvents.RaisePlayerDetected();
            RaisePrivatePlayerDetected(PlayerLocator.Player.position);
        }
        else if (!detected && playerInVision)
        {
            playerInVision = false;
            DetectionEvents.RaisePlayerHidden();
            RaisePrivatePlayerHidden(PlayerLocator.Player.position);
        }
    }

    bool DetectPlayer()
    {
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(~0); // all layers
        filter.useTriggers = false; // exclude triggers

        Collider2D[] hits = new Collider2D[20];
        int count = Physics2D.OverlapCircle(transform.position, length, filter, hits);

        for (int i = 0; i < count; i++)
        {
            var hit = hits[i];
            if (PlayerLocator.IsPlayer(hit.transform))
            {
                Vector2 dirToTarget = (hit.transform.position - transform.position).normalized;
                float angleTo = Vector2.Angle(transform.right, dirToTarget);

                if (angleTo <= angle)
                {
                    RaycastHit2D rayHit = Physics2D.Raycast(
                        transform.position,
                        dirToTarget,
                        length,
                        visionObstacles
                    );
                    if (rayHit.collider != null && PlayerLocator.IsPlayer(rayHit.collider.transform))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector3 leftDir = Quaternion.Euler(0, 0, angle) * transform.right;
        Vector3 rightDir = Quaternion.Euler(0, 0, -angle) * transform.right;
        Gizmos.DrawLine(transform.position, transform.position + leftDir * length);
        Gizmos.DrawLine(transform.position, transform.position + rightDir * length);
        Gizmos.DrawWireSphere(transform.position, length);
    }

    void RaisePrivatePlayerDetected(Vector3 position)
    {
        OnPlayerDetected?.Invoke(position);
    }
    void RaisePrivatePlayerHidden(Vector3 position) 
    {
        OnPlayerHidden?.Invoke(position);
    }
}
