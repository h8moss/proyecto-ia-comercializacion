using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WASDKeyAnimation : MonoBehaviour
{
    [Header("Teclas")]
    [SerializeField] private Image keyW;
    [SerializeField] private Image keyA;
    [SerializeField] private Image keyS;
    [SerializeField] private Image keyD;

    [Header("Colores")]
    [SerializeField] private Color normalColor = new Color(1f, 1f, 1f, 1f);
    [SerializeField] private Color pressedColor = new Color(1f, 0.77f, 0.09f, 1f);

    [Header("Tiempos")]
    [SerializeField] private float pressDuration = 0.3f;
    [SerializeField] private float delayBetween = 0.2f;

    void OnEnable()
    {
        StartCoroutine(AnimateKeys());
    }

    IEnumerator AnimateKeys()
    {
        while (true)
        {
            yield return PressKey(keyW);
            yield return new WaitForSecondsRealtime(delayBetween);
            yield return PressKey(keyA);
            yield return new WaitForSecondsRealtime(delayBetween);
            yield return PressKey(keyS);
            yield return new WaitForSecondsRealtime(delayBetween);
            yield return PressKey(keyD);
            yield return new WaitForSecondsRealtime(delayBetween * 2);
        }
    }

    IEnumerator PressKey(Image key)
    {
        if (key == null) yield break;
        key.color = pressedColor;
        yield return new WaitForSecondsRealtime(pressDuration);
        key.color = normalColor;
    }
}