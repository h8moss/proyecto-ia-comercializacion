using System;
using UnityEngine;

[RequireComponent(typeof(OceanManager))]
[RequireComponent(typeof(GeneralEnemyRotation))]
public class StaticBehaviour : MonoBehaviour
{
    [Header("Sound Detection")]
    [SerializeField] private float maxHearingDistance = 15f;

    [Header("Idle Behaviour")]
    [SerializeField] private float idleReturnDelay = 3f;
    [SerializeField] private float idleLookInterval = 4f;
    [SerializeField] private float idleLookAngleRange = 90f;

    public event Action<Vector2, float> OnSoundHeard;
    public event Action OnReturnedToIdle;

    private OceanManager ocean;
    private GeneralEnemyRotation rotator;
    private float idleReturnTimer = 0f;
    private float idleLookTimer = 0f;
    private bool isAlert = false;
    private Quaternion defaultRotation;

    void Start()
    {
        ocean = GetComponent<OceanManager>();
        rotator = GetComponent<GeneralEnemyRotation>();
        WorldEvents.OnSoundMade += HandleSoundMade;
        idleLookTimer = GetNextLookInterval();

        defaultRotation = transform.rotation;
    }

    void OnDestroy()
    {
        WorldEvents.OnSoundMade -= HandleSoundMade;
    }

    void Update()
    {
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
            rotator.TargetRotation = defaultRotation;
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
                rotator.LookAtRandomAngle(defaultRotation, idleLookAngleRange);

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
            rotator.LookAt(position);

        float alertDuration = idleReturnDelay * (1f + ocean.Conscientiousness);
        idleReturnTimer = Mathf.Max(idleReturnTimer, alertDuration);
        isAlert = true;

        OnSoundHeard?.Invoke(position, perceivedLoudness);
    }

    float GetNextLookInterval()
    {
        float lookDrive = (ocean.Openness + ocean.Neuroticism) / 2f;
        return idleLookInterval * (1f - lookDrive * 0.6f);
    }
}
