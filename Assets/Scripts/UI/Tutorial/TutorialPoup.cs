using UnityEngine;

public class TutorialPopup : MonoBehaviour
{
    [SerializeField] private GameObject popupRoot;
    [SerializeField] private KeyCode closeKey = KeyCode.Space;
    [SerializeField] private bool showOnStart = true;  // NUEVO

    private bool isOpen = false;

    void Start()
    {
        if (popupRoot != null)
            popupRoot.SetActive(false);

        if (showOnStart)
            Show();  // NUEVO: aparece automáticamente
    }
    void Update()
    {
        if (isOpen && (Input.GetKeyDown(KeyCode.W) || 
                    Input.GetKeyDown(KeyCode.A) || 
                    Input.GetKeyDown(KeyCode.S) || 
                    Input.GetKeyDown(KeyCode.D)))
        {
            Hide();
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