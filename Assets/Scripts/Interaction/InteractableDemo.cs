using UnityEngine;

[RequireComponent(typeof(Interactable))]
public class InteractableDemo : MonoBehaviour
{
    private Interactable interactable;

    void Start()
    {
        interactable = GetComponent<Interactable>();
        interactable.onInteraction += OnInteraction;
    }

    void OnInteraction()
    {
        Debug.Log("Interacted!");
    }
}
