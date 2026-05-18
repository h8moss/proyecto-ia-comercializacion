using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    // Reference to the pause menu UI panel
    [SerializeField] private GameObject MenuPause;

    private static bool paused = false;
    public static bool Paused { get => paused; }

    void Update()
    {
        // Toggle pause when the Pause button is pressed
        if (Input.GetButtonDown("Pause"))
        {
            if (paused)
                Resume();
            else
                Pausa();
        }
    }

    // Pauses the game and shows the pause menu
    public void Pausa()
    {
        paused = true;
        Time.timeScale = 0f;
        MenuPause.SetActive(true);
    }

    // Resumes the game and hides the pause menu
    public void Resume()
    {
        paused = false;
        Time.timeScale = 1f;
        MenuPause.SetActive(false);
    }

    // Returns to the main menu and resets the game state
    public void LoadMenu()
    {
        Time.timeScale = 1f;
        paused = false;
        SceneManager.LoadScene("SelectGame");
    }
}