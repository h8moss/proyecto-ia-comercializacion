using UnityEngine;
using UnityEngine.UI;

public class IndicatorBlink : MonoBehaviour
{
    [SerializeField] private float blinkSpeed = 1f;

    private Image img;

    void Start()
    {
        img = GetComponent<Image>();
    }

    void Update()
    {
        float alpha = Mathf.Abs(Mathf.Sin(Time.unscaledTime * blinkSpeed * Mathf.PI));
        Color c = img.color;
        c.a = alpha;
        img.color = c;
    }
}