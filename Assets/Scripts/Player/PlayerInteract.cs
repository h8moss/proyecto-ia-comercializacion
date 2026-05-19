using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    private PlayerDetection playerDetection;
    private HidingBehaviour hb;

    Interactable currentInteractable = null;
    bool canInteract = true;

    void Start()
    {
        playerDetection = GetComponent<PlayerDetection>();
        hb = GetComponent<HidingBehaviour>();
        playerDetection.OnDeath += Deactivate;
        hb.OnHidden += Deactivate;
        hb.OnUnhidden += Reactivate; 
    }

    void Update()
    {
        if (currentInteractable != null)
        {
            if (Input.GetButtonDown("Interact") && canInteract)
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

    void Deactivate()
    {
        canInteract = false;
    }

    void Reactivate()
    {
        canInteract = true;
    }
}
