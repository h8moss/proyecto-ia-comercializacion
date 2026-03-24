using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(TMP_Text))]
public class ObjectivesData : MonoBehaviour
{
    private TMP_Text text;
    private ObjectiveManager manager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        text = GetComponent<TMP_Text>();
        manager = ObjectiveManager.Instance;
        manager.ObjectiveChanged += SetObjective;
        SetObjective();
    }

    void SetObjective() {
        text.text = manager.Objectives[manager.CurrentObjective].title;
    }
}
