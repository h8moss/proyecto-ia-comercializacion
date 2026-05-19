using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Manages the objectives of a level in order.
/// When all objectives are completed, saves progress and loads the next level.
/// </summary>
public class ObjectiveManager : MonoBehaviour
{
    static ObjectiveManager instance;
    public static ObjectiveManager Instance { get => instance; }

    // List of objectives for this level, assigned in order from the Inspector
    [SerializeField] private Objective[] objectives;

    // The level number this scene represents (1-based: lvl1 = 1, lvl2 = 2, etc.)
    [SerializeField] private int level;

    [SerializeField] private Image endgameCanvas;

    // Ordered list of level scene names, must match Build Settings
    private readonly string[] levelScenes = { "lvl1", "lvl2", "lvl3", "lvl4" };

    public Objective[] Objectives { get => objectives; }

    public int currentObjective = 0;
    public int CurrentObjective { get => currentObjective; }

    // Returns true if there are still pending objectives
    public bool HasObjective { get => currentObjective < objectives.Length; }

    // Fired whenever the current objective changes
    public Action ObjectiveChanged;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        // Subscribe to all objective completion events
        foreach (var obj in objectives)
        {
            obj.onCompleted += OnObjectiveCompleted;
        }

        // Activate the first objective
        objectives[0].RaiseOnBecomeActive();
    }

    /// <summary>
    /// Called when an objective is completed.
    /// Advances to the next objective or completes the level if all are done.
    /// </summary>
    void OnObjectiveCompleted(Objective o)
    {
        if (o == objectives[currentObjective])
        {
            currentObjective++;

            if (currentObjective == objectives.Length)
            {
                // All objectives completed, finish the level
                CompleteLevel();
            }
            else
            {
                // Activate the next objective
                objectives[currentObjective].RaiseOnBecomeActive();
            }

            ObjectiveChanged?.Invoke();
        }
    }

    /// <summary>
    /// Saves the player's progress and loads the next level.
    /// If there are no more levels, redirects to the level selector.
    /// </summary>
    void CompleteLevel()
    {
        SaveManager manager = SaveManager.Instance;
        manager.Save(
            manager.State.CopyWith(levelProgress: Mathf.Max(level, manager.State.LevelProgress))
        );

        if (level < levelScenes.Length)
        {
            // Check if the next level exists in Build Settings
            if (Application.CanStreamedLevelBeLoaded(levelScenes[level]))
            {
                SceneManager.LoadScene(levelScenes[level]);
            }
            else
            {
                // Next level not available yet, go to main menu
                Debug.LogWarning($"Level {levelScenes[level]} not found, redirecting to MenuLobby");
                SceneManager.LoadScene("MenuLobby");
            }
        }
        else
        {
            StartCoroutine(EndOfGame());
        }
    }

    IEnumerator EndOfGame()
    {
        if (endgameCanvas != null)
        {

            endgameCanvas.gameObject.SetActive(true);
            yield return new WaitForEndOfFrame();
            endgameCanvas.GetComponent<EndGameUI>().fadeDuration = 5f;
            endgameCanvas.GetComponent<EndGameUI>().StartFade();
            yield return new WaitForSeconds(8);
            SceneManager.LoadScene(0);
        }
    }
}