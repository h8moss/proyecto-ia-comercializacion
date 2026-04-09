using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;

public class MenuSystem : MonoBehaviour
{
  
    public void Play()
    {
        int maxLevel = 4; // TODO: Change this to the real max level? Figure out a better implementation
        int levelToLoad = Mathf.Min(maxLevel, SaveManager.Instance.State.LevelProgress+1);

        // We add one because scene 0 is the main menu and scene 1 is the level menu, so level 1 is scene 2.
        SceneManager.LoadScene(levelToLoad + 1);
    }

    public void SelectLevels()
    {
        SceneManager.LoadScene(1);
    }

    public void Exit()
    {
        Debug.Log("Saliste del juego...");
        Application.Quit();
    }
}
