using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class DisableLevelButton : MonoBehaviour
{
    [SerializeField] private int level;

    void Start()
    {
        Button btn = GetComponent<Button>();
        int currentLevel = SaveManager.Instance.State.LevelProgress + 1;

        btn.interactable = level <= currentLevel;

        Debug.Log(SaveManager.Instance.State.LevelProgress);
    }
}
