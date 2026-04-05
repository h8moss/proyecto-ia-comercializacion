using UnityEngine;

public class StepsSound : MonoBehaviour
{
    [SerializeField] private float stepLength;
    [SerializeField] private float stepLoudness;
    private float distanceWalked;
    private Vector3 lastPos;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lastPos = transform.position;
        TakeStep();
    }

    // Update is called once per frame
    void Update()
    {
        float deltaDistance = Vector3.Distance(lastPos, transform.position);
        distanceWalked += deltaDistance;

        if (distanceWalked > stepLength)
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
}
