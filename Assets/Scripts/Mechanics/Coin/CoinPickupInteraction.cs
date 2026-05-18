using UnityEngine;

[RequireComponent(typeof(Interactable))]
public class CoinPickupInteraction : MonoBehaviour
{
    private Interactable interactable;

    void Start()
    {
        interactable = GetComponent<Interactable>();
        interactable.onInteraction += OnInteracted;
    }

    void OnDestroy()
    {
        interactable.onInteraction -= OnInteracted;
    }

    void OnInteracted()
    {
        PlayerLocator.Player.GetComponent<PlayerThrowCoin>().PickedupCoin();

        Destroy(transform.parent.gameObject);
    }

    public void OnBBEGInteracted()
    {
        Destroy(transform.parent.gameObject);
    }
}
