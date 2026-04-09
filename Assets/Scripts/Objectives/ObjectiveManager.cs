using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectiveManager : MonoBehaviour
{
    static ObjectiveManager instance;
    public static ObjectiveManager Instance { get => instance; }
    [SerializeField] private Objective[] objectives;
    [SerializeField] private int level;

    public Objective[] Objectives { get => objectives; }

    public int currentObjective = 0;
    public int CurrentObjective { get => currentObjective; }
    public bool HasObjective { get => currentObjective < objectives.Length; }

    public Action ObjectiveChanged;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        foreach (var obj in objectives)
        {
            obj.onCompleted += OnObjectiveCompleted;
        }
        objectives[0].RaiseOnBecomeActive();
    }

    void OnObjectiveCompleted(Objective o)
    {
        if (o == objectives[currentObjective])
        {
            // o.CompleteObjective();
            currentObjective++;
            if (currentObjective == objectives.Length)
            {
                CompleteLevel();
            } else
            {
                objectives[currentObjective].RaiseOnBecomeActive();
            }
            ObjectiveChanged?.Invoke();
        }
    }

    void CompleteLevel()
    {
        SaveManager manager = SaveManager.Instance;
        manager.Save(
            manager.State.CopyWith(levelProgress: Mathf.Max(level, manager.State.LevelProgress))
        );

        // Load level selector
        SceneManager.LoadScene(1);
    }
}
