using UnityEngine;

public class StepsSound : MonoBehaviour
{
    [SerializeField] private float stepLength;
    [SerializeField] private float stepLoudness;
    private float distanceWalked;
    private Vector3 lastPos;

    private bool isHidden;

    void Start()
    {
        GetComponent<HidingBehaviour>().OnHidden += OnHidden;
        GetComponent<HidingBehaviour>().OnUnhidden += OnUnhidden;
        
        lastPos = transform.position;
        TakeStep();
    }

    void OnDestroy()
    {
        GetComponent<HidingBehaviour>().OnHidden -= OnHidden;
        GetComponent<HidingBehaviour>().OnUnhidden -= OnUnhidden;
    }

    void Update()
    {
        float deltaDistance = Vector3.Distance(lastPos, transform.position);
        distanceWalked += deltaDistance;

        if (distanceWalked > stepLength && !isHidden)
        {
            TakeStep();
        }

        lastPos = transform.position;
    }

    void TakeStep()
    {
        distanceWalked = 0;
        WorldEvents.RaiseSoundMade(transform.position, stepLoudness);
    }

    void OnHidden()
    {
        isHidden = true;
    }

    void OnUnhidden()
    {
        isHidden = false;
    }
}
