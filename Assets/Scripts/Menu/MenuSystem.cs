using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;

public class MenuSystem : MonoBehaviour
{
    // Redirects to the game selection screen (New Game / Continue)
    public void Play()
    {
        SceneManager.LoadScene("SelectGame");
    }

    // Redirects to the level selection screen
    public void SelectLevels()
    {
        SceneManager.LoadScene("SelectLevel");
    }

    // Exits the application
    public void Exit()
    {
        Debug.Log("Saliste del juego...");
        Application.Quit();
    }
}