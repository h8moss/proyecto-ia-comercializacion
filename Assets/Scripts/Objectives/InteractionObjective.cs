using UnityEngine;

[RequireComponent(typeof(Objective))]
[RequireComponent(typeof(Interactable))]
public class InteractionObjective : MonoBehaviour
{

    private Objective objective;
    private Interactable interactable;

    void Start()
    {
        objective = GetComponent<Objective>();
        interactable = GetComponent<Interactable>();

        objective.onCompleted += Completed;
        interactable.onInteraction += Interacted;
    }

    void Completed(Objective _)
    {
        interactable.enabled = false;
    }

    void Interacted()
    {
        if (objective.IsActiveObjective)
        {
            objective.CompleteObjective();
        }
    }
}
