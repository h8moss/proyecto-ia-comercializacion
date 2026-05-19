using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TutorialTrigger : MonoBehaviour
{
    [SerializeField] private TutorialPopup popup;
    [SerializeField] private bool onlyOnce = true;

    private bool triggered = false;

    void Reset()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Algo entró al trigger: " + other.name + " con tag: " + other.tag);
        
        if (triggered && onlyOnce) return;
        if (!other.CompareTag("Player")) 
        {
            Debug.Log("No es Player, ignorando");
            return;
        }

        if (popup != null)
        {
            Debug.Log("Mostrando popup");
            popup.Show();
            triggered = true;
        }
        else
        {
            Debug.Log("Popup es null!");
        }
    }
}