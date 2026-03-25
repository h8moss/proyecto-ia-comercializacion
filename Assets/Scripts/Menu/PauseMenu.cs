using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;


public class PauseMenu : MonoBehaviour
{

    [SerializeField] private GameObject ButtonPause;

    [SerializeField] private GameObject MenuPause;

   public void Pausa()
    {
        Time.timeScale = 0f;
        ButtonPause.SetActive(false);
        MenuPause.SetActive(true);

    }

    public void Resume()
    {
        Time.timeScale = 1f;
        ButtonPause.SetActive(true);
        MenuPause.SetActive(false);
    }
    
    public void Menu (String nombre)
    {
        SceneManager.LoadScene(nombre);
    }
}
