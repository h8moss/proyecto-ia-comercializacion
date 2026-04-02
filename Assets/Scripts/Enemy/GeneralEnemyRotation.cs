using UnityEngine;

public class GeneralEnemyRotation : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 90f;
    [SerializeField] private float forwardAngleOffset = -90f;
 
    private Quaternion targetRotation;
 
    public Quaternion TargetRotation
    {
        get => targetRotation;
        set => targetRotation = value;
    }

    void Start()
    {
        targetRotation = transform.rotation;
    }
 
    void Update()
    {
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }
 
    /// <summary>
    /// Points the character towards a world-space position.
    /// </summary>
    public void LookAt(Vector2 target)
    {
        targetRotation = GetRotationTowards(target);
    }
 
    /// <summary>
    /// Applies a random angular offset from a rotation, clamped to +/- angleRange degrees.
    /// </summary>
    public void LookAtRandomAngle(Quaternion rotation, float angleRange)
    {
        float angle = Random.Range(-angleRange, angleRange);
        targetRotation = rotation * Quaternion.Euler(0f, 0f, angle);
    }
 
    private Quaternion GetRotationTowards(Vector2 target)
    {
        Vector2 direction = (target - (Vector2)transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        return Quaternion.Euler(0f, 0f, angle + forwardAngleOffset);
    }


}
