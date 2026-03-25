using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;

public class MenuSystem : MonoBehaviour
{
  
    public void Play()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
    }

    public void SelectLevels (String nombre)
    {
        SceneManager.LoadScene(nombre);
    }

    public void Exit()
    {
        Debug.Log("Saliste del juego...");
        Application.Quit();
    }
}
