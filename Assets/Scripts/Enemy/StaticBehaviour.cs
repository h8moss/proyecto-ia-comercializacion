using System;
using UnityEngine;

[RequireComponent(typeof(OceanManager))]
public class StaticBehaviour : MonoBehaviour
{
    [Header("Sound Detection")]
    [SerializeField] private float maxHearingDistance = 15f;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 90f;

    [Header("Idle Behaviour")]
    [SerializeField] private float idleReturnDelay = 3f;
    [SerializeField] private float idleLookInterval = 4f;
    [SerializeField] private float idleLookAngleRange = 90f;

    public event Action<Vector2, float> OnSoundHeard;
    public event Action OnReturnedToIdle;

    private OceanManager ocean;
    private Quaternion defaultRotation;
    private Quaternion targetRotation;
    private float idleReturnTimer = 0f;
    private float idleLookTimer = 0f;
    private bool isAlert = false;

    void Start()
    {
        ocean = GetComponent<OceanManager>();
        defaultRotation = transform.rotation;
        targetRotation = defaultRotation;
        WorldEvents.OnSoundMade += HandleSoundMade;
        idleLookTimer = GetNextLookInterval();
    }

    void OnDestroy()
    {
        WorldEvents.OnSoundMade -= HandleSoundMade;
    }

    void Update()
    {
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );

        if (isAlert)
            UpdateAlert();
        else
            UpdateIdle();
    }

    void UpdateAlert()
    {
        idleReturnTimer -= Time.deltaTime;
        if (idleReturnTimer <= 0f)
        {
            isAlert = false;
            targetRotation = defaultRotation;
            idleLookTimer = GetNextLookInterval();
            OnReturnedToIdle?.Invoke();
        }
    }

    void UpdateIdle()
    {
        float lookDrive = (ocean.Openness + ocean.Neuroticism) / 2f;
        if (lookDrive < 0.3f) return;

        idleLookTimer -= Time.deltaTime;
        if (idleLookTimer <= 0f)
        {
            if (UnityEngine.Random.value < lookDrive)
            {
                float angle = UnityEngine.Random.Range(-idleLookAngleRange, idleLookAngleRange);
                targetRotation = defaultRotation * Quaternion.Euler(0f, 0f, angle);
            }
            idleLookTimer = GetNextLookInterval();
        }
    }

    void HandleSoundMade(Vector2 position, float initialLoudness)
    {
        float dst = Vector2.Distance(transform.position, position);
        if (dst > maxHearingDistance) return;

        float perceivedLoudness = initialLoudness / (dst * dst);

        float threshold = 0.5f - (ocean.Neuroticism * 0.4f);
        if (perceivedLoudness < threshold) return;

        if (UnityEngine.Random.value < ocean.Openness)
            targetRotation = GetRotationTowards(position);

        float alertDuration = idleReturnDelay * (1f + ocean.Conscientiousness);
        idleReturnTimer = Mathf.Max(idleReturnTimer, alertDuration);
        isAlert = true;

        OnSoundHeard?.Invoke(position, perceivedLoudness);
    }

    Quaternion GetRotationTowards(Vector2 target)
    {
        Vector2 direction = (target - (Vector2)transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        return Quaternion.Euler(0f, 0f, angle - 90f);
    }

    float GetNextLookInterval()
    {
        float lookDrive = (ocean.Openness + ocean.Neuroticism) / 2f;
        return idleLookInterval * (1f - lookDrive * 0.6f);
    }
}
