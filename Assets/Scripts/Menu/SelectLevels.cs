using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SelectLevels : MonoBehaviour
{
    public void ChangeLevel(string nameLevel)
    {
        SceneManager.LoadScene(nameLevel);

    }

    public void ChangeLevel(int nameLevel)
    {
        SceneManager.LoadScene(nameLevel);
    }
    public void Menu (String nombre)
    {
        SceneManager.LoadScene(nombre);
    }
}
