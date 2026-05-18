using UnityEngine;

[RequireComponent(typeof(OceanManager))]
public class OceanRandomizer : MonoBehaviour
{
    [SerializeField] private int seed = 0;

    private OceanManager ocean;
    void Start()
    {
        ocean = GetComponent<OceanManager>();
        var rng = new System.Random(seed);

        ocean.setO((float)rng.NextDouble());
        ocean.setC((float)rng.NextDouble());
        ocean.setE((float)rng.NextDouble());
        ocean.setA((float)rng.NextDouble());
        ocean.setN((float)rng.NextDouble());
    }
}
