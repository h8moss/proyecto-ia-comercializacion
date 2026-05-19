using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    private PlayerDetection playerDetection;
    private HidingBehaviour hb;

    List<Interactable> currentInteractable = new();
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
        if (currentInteractable.Count > 0)
        {
            if (Input.GetButtonDown("Interact") && canInteract)
            {
                foreach (var i in currentInteractable)
                {
                    i.onInteraction?.Invoke();
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Interactable>(out var interactable))
        {
            currentInteractable.Add(interactable);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        int toPop = -1;
        for (int i=0; i<currentInteractable.Count; i++)
        {
            if (currentInteractable[i].gameObject == collision.gameObject)
            {
                toPop = i;
                break;
            }
        }
        if (toPop > -1)
        {
            currentInteractable.RemoveAt(toPop);
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
