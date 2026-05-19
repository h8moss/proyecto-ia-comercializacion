using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectGame : MonoBehaviour
{
    // Reference to the Continue button to enable/disable it based on save data
    [SerializeField] private Button continueButton;

    // Ordered list of level scene names
    private readonly string[] levelScenes = { "lvl1", "lvl2", "lvl3", "lvl4" };

    private void Start()
    {
        // Disable Continue button if there is no saved progress
        bool hasSave = SaveManager.Instance.State.LevelProgress > 0;
        continueButton.interactable = hasSave;
    }

    // Resets save data and starts from the first level
    public void NewGame()
    {
        SaveManager.Instance.Save(SaveState.Defaults());
        SceneManager.LoadScene("lvl1");
    }

    // Loads the last level the player reached based on saved progress
    public void Continue()
    {
        int progress = SaveManager.Instance.State.LevelProgress;
        int index = Mathf.Clamp(progress, 0, levelScenes.Length - 1);
        SceneManager.LoadScene(levelScenes[index]);
    }

    // Loads a menu scene by name
    public void Menu(string nombre)
    {
        SceneManager.LoadScene(nombre);
    }
}