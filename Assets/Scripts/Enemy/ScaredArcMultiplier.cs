using System;
using UnityEngine;

[RequireComponent(typeof(DetectionArc))]
public class ScaredArcMultiplier : MonoBehaviour
{
    [SerializeField] private float lengthMultiplier;
    [SerializeField] private float angleMultiplier;

    private DetectionArc arc;


    private float initialAngle;
    private float initialLength;

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
        if (scared)
        {
            arc.Angle = arc.Angle * angleMultiplier;
            arc.Length = arc.Length * lengthMultiplier;
        } else
        {
            arc.Angle =  initialAngle;
            arc.Length = initialLength;
        }
    }
}
