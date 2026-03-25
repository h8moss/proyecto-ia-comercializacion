using UnityEngine;

[RequireComponent(typeof(Objective))]
public class LocationObjective : MonoBehaviour
{
    private Objective objective;

    void Start()
    {
        objective = GetComponent<Objective>();
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        objective.CompleteObjective();
    }
}
