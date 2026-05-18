using UnityEngine;
using System.Collections;

[RequireComponent(typeof(DetectionArc))]
public class ScaredArcMultiplier : MonoBehaviour
{
    [SerializeField] private float lengthMultiplier;
    [SerializeField] private float angleMultiplier;
    [SerializeField] private float transitionDuration = 0.5f;

    private DetectionArc arc;
    private float initialAngle;
    private float initialLength;

    private Coroutine transitionCoroutine;

    void Start()
    {
        arc = GetComponent<DetectionArc>();
        GetComponent<ScareableEnemy>().ScaredChanged += OnScaredChanged;
        initialAngle = arc.Angle;
        initialLength = arc.Length;
    }

    void OnDestroy()
    {
        GetComponent<ScareableEnemy>().ScaredChanged -= OnScaredChanged;
    }

    void OnScaredChanged(bool scared)
    {
        float targetAngle  = scared ? initialAngle * angleMultiplier  : initialAngle;
        float targetLength = scared ? initialLength * lengthMultiplier : initialLength;

        if (transitionCoroutine != null)
            StopCoroutine(transitionCoroutine);

        transitionCoroutine = StartCoroutine(TransitionArc(targetAngle, targetLength));
    }

    private IEnumerator TransitionArc(float targetAngle, float targetLength)
    {
        float elapsed      = 0f;
        float startAngle   = arc.Angle;
        float startLength  = arc.Length;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t  = Mathf.Clamp01(elapsed / transitionDuration);
            float st = Mathf.SmoothStep(0f, 1f, t);  // ease in/out

            arc.Angle  = Mathf.Lerp(startAngle,  targetAngle,  st);
            arc.Length = Mathf.Lerp(startLength, targetLength, st);

            yield return null;
        }

        // Snap to exact target at the end to avoid floating point drift
        arc.Angle  = targetAngle;
        arc.Length = targetLength;
        transitionCoroutine = null;
    }
}
