using UnityEngine;

public class ShowRandomChild : MonoBehaviour
{
    [Range(0.0f, 1.0f)]
    [SerializeField] private float ChildAProbability = 0.5f;

    [SerializeField] private GameObject childA;
    [SerializeField] private GameObject childB;

    void Start()
    {
        bool result = Random.value < ChildAProbability;
        childA.SetActive(result);
        childB.SetActive(!result);
    }
}
