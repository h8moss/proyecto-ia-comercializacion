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
        Debug.Log("[" + gameObject.name + "] ENTRÓ: " + other.name + " tag: " + other.tag);
        
        if (triggered && onlyOnce) 
        {
            Debug.Log("Ya fue triggered y onlyOnce activo, salgo");
            return;
        }
        if (!PlayerLocator.IsPlayer(other.transform)) 
        {
            Debug.Log("No es Player");
            return;
        }

        if (popup != null)
        {
            Debug.Log("Mostrando popup en " + gameObject.name);
            StartCoroutine(popup.Show());
            triggered = true;
        }
        else
        {
            Debug.Log("Popup es NULL");
        }
    }
}