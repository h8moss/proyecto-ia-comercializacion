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

    public void LoadLevel1()
    {
        SceneManager.LoadScene(2);
    }

    public void LoadLevel2()
    {
        SceneManager.LoadScene(3);
    }

    public void LoadLevel3()
    {
        Debug.LogWarning("Level 3 is not connected");
    }

    public void LoadLevel4()
    {
        Debug.LogWarning("Level 4 is not connected");
    }

}
