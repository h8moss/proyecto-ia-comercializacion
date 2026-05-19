using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class ObjectivesData : MonoBehaviour
{
    private TMP_Text text;
    private ObjectiveManager manager;

    [Header("Parpadeo")]
    [SerializeField] private int blinkCount = 6;
    [SerializeField] private float blinkSpeed = 0.1f;
    [SerializeField] private float fadeOutSpeed = 0.5f;

    void Start()
    {
        text = GetComponent<TMP_Text>();
        manager = ObjectiveManager.Instance;
        manager.ObjectiveChanged += OnObjectiveChanged;

        StartCoroutine(InitMission());
    }

    IEnumerator InitMission()
    {
        yield return null;
        SetObjective();
        yield return StartCoroutine(ShowMission());
    }

    void OnObjectiveChanged()
    {
        StartCoroutine(FadeAndChange());
    }

    IEnumerator FadeAndChange()
    {
        // Fade out del texto actual
        float t = 1f;
        while (t > 0f)
        {
            t -= Time.deltaTime / fadeOutSpeed;
            text.alpha = Mathf.Clamp01(t);
            yield return null;
        }

        // Cambiar texto
        SetObjective();

        // Parpadear texto nuevo
        yield return StartCoroutine(ShowMission());
    }

    IEnumerator ShowMission()
    {
        for (int i = 0; i < blinkCount; i++)
        {
            text.alpha = (i % 2 == 0) ? 1f : 0f;
            yield return new WaitForSeconds(blinkSpeed);
        }
        text.alpha = 1f;
    }

    void SetObjective()
    {
        if (manager.HasObjective)
            text.text = manager.Objectives[manager.CurrentObjective].title;
        else
            text.text = "";
    }
}