using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    Interactable currentInteractable = null;

    void Update()
    {
        if (currentInteractable != null)
        {
            if (Input.GetButtonDown("Interact"))
            {
                currentInteractable.onInteraction?.Invoke();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Interactable>(out var interactable))
        {
            currentInteractable = interactable;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (currentInteractable != null && collision.gameObject == currentInteractable.gameObject)
        {
            currentInteractable = null;
        }
    }
}
