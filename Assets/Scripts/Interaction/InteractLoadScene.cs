using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Interactable))]
public class InteractLoadScene : MonoBehaviour
{
    private Interactable interactable;
    [SerializeField] private int scene;

    void Start()
    {
        interactable = GetComponent<Interactable>();
        interactable.onInteraction += OnInteraction;
    }

    void OnInteraction()
    {
        SceneManager.LoadScene(scene);
    }
}
