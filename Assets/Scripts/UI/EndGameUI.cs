using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndGameUI : MonoBehaviour
{
    public TMP_Text text;
    private Image img;

    public bool fadeOut;
    public float fadeDuration;

    private float timeElapsed = 0;
    private bool isFading;

    void Start()
    {
        img = GetComponent<Image>();
    }

    void Update()
    {
        if (isFading)
        {
            timeElapsed += Time.deltaTime;
            float val = Mathf.Clamp(timeElapsed / fadeDuration, 0, 1);
            if (fadeOut)
            {
                val = 1-val;
            }

            text.color = new(
                a: val,
                r: text.color.r,
                g: text.color.g,
                b: text.color.b
            );
            img.color = new(
                a: val,
                r: img.color.r,
                g: img.color.g,
                b: img.color.b
            );
        }
    }

    public void StartFade()
    {
        timeElapsed = 0;
        isFading = true;
    }
}
