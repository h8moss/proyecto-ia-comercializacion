using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider2D))]
public class IndicatorBlink : MonoBehaviour
{
    [SerializeField] private float blinkSpeed = 1f;
    [SerializeField] private KeyCode interactKey = KeyCode.F;

    private Image img;
    private bool playerNear = false;

    void Start()
    {
        img = GetComponent<Image>();
    }

    void Update()
    {
        // Parpadeo
        float alpha = Mathf.Abs(Mathf.Sin(Time.unscaledTime * blinkSpeed * Mathf.PI));
        Color c = img.color;
        c.a = alpha;
        img.color = c;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerNear = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerNear = false;
    }
}