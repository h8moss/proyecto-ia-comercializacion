using UnityEngine;

public class TutorialPopup : MonoBehaviour
{
    [SerializeField] private GameObject popupRoot;
    [SerializeField] private bool showOnStart = true;
    [SerializeField] private KeyCode[] closeKeys; // configurable desde Inspector

    private bool isOpen = false;

    void Start()
    {
        if (popupRoot != null)
            popupRoot.SetActive(false);

        if (showOnStart)
            Show();
    }
    void Update()
    {
        if (!isOpen) return;

        // Cierre con teclas configuradas
        foreach (KeyCode key in closeKeys)
        {
            if (Input.GetKeyDown(key))
            {
                Hide();
                return;
            }
        }

        // Cierre con click izquierdo o derecho
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            Hide();
            return;
        }
    }

    public void Show()
    {
        if (popupRoot != null)
        {
            popupRoot.SetActive(true);
            isOpen = true;
            Time.timeScale = 0f;
        }
    }

    public void Hide()
    {
        if (popupRoot != null)
        {
            popupRoot.SetActive(false);
            isOpen = false;
            Time.timeScale = 1f;
        }
    }
}