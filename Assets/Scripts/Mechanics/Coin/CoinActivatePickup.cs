using UnityEngine;

[RequireComponent(typeof(CoinController))]
public class CoinActivatePickup : MonoBehaviour
{
    [SerializeField] private GameObject pickupGO;

    private CoinController controller;
    void Start()
    {
        controller = GetComponent<CoinController>();
        controller.OnLanded += OnLanded;
    }

    void OnDestroy()
    {
        controller.OnLanded -= OnLanded;
    }

    void OnLanded()
    {
        pickupGO.SetActive(true);
    }
}
