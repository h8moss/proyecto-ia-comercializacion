using UnityEngine;
using UnityEngine.UI;

// Disables a level button if the player has not reached that level yet
[RequireComponent(typeof(Button))]
public class DisableLevelButton : MonoBehaviour
{
    // The level number this button represents
    [SerializeField] private int level;

    void Start()
    {
        Button btn = GetComponent<Button>();

        // Allow access to levels the player has already unlocked
        int currentLevel = SaveManager.Instance.State.LevelProgress + 1;
        btn.interactable = level <= currentLevel;
    }
}