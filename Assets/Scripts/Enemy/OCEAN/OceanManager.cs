using UnityEngine;

public class OceanManager : MonoBehaviour
{
    [SerializeField, Range(0f, 1f)]
    private float openness;
    [SerializeField, Range(0f, 1f)]
    private float conscientiousness;
    [SerializeField, Range(0f, 1f)]
    private float extraversion;
    [SerializeField, Range(0f, 1f)]
    private float agreeableness;
    [SerializeField, Range(0f, 1f)]
    private float neuroticism;

    public float Openness { get => openness; }
    public float Conscientiousness { get => conscientiousness; }
    public float Extraversion { get => extraversion; }
    public float Agreeableness { get => agreeableness; }
    public float Neuroticism { get => neuroticism; }

    public float O { get => openness; }
    public float C { get => conscientiousness; }
    public float E { get => extraversion; }
    public float A { get => agreeableness; }
    public float N { get => neuroticism; }

    public void setO(float o) => openness = o;
    public void setC(float c) => conscientiousness = c;
    public void setE(float e) => extraversion = e;
    public void setA(float a) => agreeableness = a;
    public void setN(float n) => neuroticism = n;

}
