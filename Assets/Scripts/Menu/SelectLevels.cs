using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SelectLevels : MonoBehaviour
{
    // Loads a scene by name
    public void ChangeLevel(string nameLevel)
    {
        SceneManager.LoadScene(nameLevel);
    }

    // Loads a scene by index
    public void ChangeLevel(int nameLevel)
    {
        SceneManager.LoadScene(nameLevel);
    }

    // Loads a menu scene by name
    public void Menu(string nombre)
    {
        SceneManager.LoadScene(nombre);
    }

    // Loads Level 1
    public void LoadLevel1()
    {
        SceneManager.LoadScene("lvl1");
    }

    // Loads Level 2
    public void LoadLevel2()
    {
        SceneManager.LoadScene("lvl2");
    }

    // Level 3 is not yet connected to a scene
    public void LoadLevel3()
    {
        Debug.LogWarning("Level 3 is not connected");
    }

    // Level 4 is not yet connected to a scene
    public void LoadLevel4()
    {
        Debug.LogWarning("Level 4 is not connected");
    }
}