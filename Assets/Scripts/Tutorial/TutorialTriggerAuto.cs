using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TutorialTriggerAuto : MonoBehaviour
{
    [SerializeField] private TutorialPopupAuto popup;
    [SerializeField] private bool onlyOnce = true;

    private bool triggered = false;

    void Reset()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered && onlyOnce) return;
        if (!PlayerLocator.IsPlayer(other.transform)) return;

        if (popup != null)
        {
            popup.Show();
            triggered = true;
        }
    }
}