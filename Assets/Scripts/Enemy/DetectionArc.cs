using Unity.VisualScripting;
using UnityEngine;

public class DetectionArc : MonoBehaviour
{
    [SerializeField] private float length;
    [SerializeField] private float angle;
    [SerializeField] private LayerMask visionObstacles;

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
        }
        else if (!detected && playerInVision)
        {
            playerInVision = false;
            DetectionEvents.RaisePlayerHidden();
        }
    }

    bool DetectPlayer()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, length);

        foreach (var hit in hits)
        {
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
}
