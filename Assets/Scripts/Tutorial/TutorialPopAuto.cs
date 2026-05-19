using System.Collections;
using UnityEngine;

public class TutorialPopupAuto : MonoBehaviour
{
    [SerializeField] private GameObject popupRoot;
    [SerializeField] private bool showOnStart = false;
    [SerializeField] private float autoCloseTime = 3f;

    private bool isOpen = false;

    void Start()
    {
        if (popupRoot != null)
            popupRoot.SetActive(false);

        if (showOnStart)
            Show();
    }

    public void Show()
    {
        if (popupRoot != null)
        {
            popupRoot.SetActive(true);
            isOpen = true;
            Time.timeScale = 0f;
            StartCoroutine(AutoCloseRoutine());
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

    IEnumerator AutoCloseRoutine()
    {
        yield return new WaitForSecondsRealtime(autoCloseTime);
        if (isOpen) Hide();
    }
}