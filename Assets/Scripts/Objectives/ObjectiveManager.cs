using System;
using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{
    static ObjectiveManager instance;
    public static ObjectiveManager Instance { get => instance; }
    [SerializeField] private Objective[] objectives;

    public Objective[] Objectives { get => objectives; }

    public int currentObjective = 0;
    public int CurrentObjective { get => currentObjective; }

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
                // TODO: Level complete!
            } else
            {
                objectives[currentObjective].RaiseOnBecomeActive();
            }
            ObjectiveChanged?.Invoke();
        }
    }
}
