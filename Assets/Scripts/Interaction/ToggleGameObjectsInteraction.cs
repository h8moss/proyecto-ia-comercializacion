using UnityEngine;

[RequireComponent(typeof(Interactable))]
public class ToggleGameObjects : MonoBehaviour
{
    [SerializeField] private GameObject[] objects;
    private Interactable interactable;
    
    void Start()
    {
        interactable = GetComponent<Interactable>();
        interactable.onInteraction += Toggle;
    }

    void Toggle()
    {
        foreach (var ob in objects)
        {
            ob.SetActive(!ob.activeInHierarchy);
        }
    }
}
