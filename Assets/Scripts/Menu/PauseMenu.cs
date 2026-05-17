using UnityEngine;
using UnityEngine.SceneManagement;


public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject MenuPause;

    private static bool paused = false;
    public static bool Paused { get => paused; }

    void Update()
    {
        if (Input.GetButtonDown("Pause"))
        {
            if (paused)
                Resume();
            else
                Pausa();
        }
    }

    public void Pausa()
    {
        paused = true;
        Time.timeScale = 0f;
        MenuPause.SetActive(true);
    }

    public void Resume()
    {
        paused = false;
        Time.timeScale = 1f;
        MenuPause.SetActive(false);
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene(0);
    }
}
