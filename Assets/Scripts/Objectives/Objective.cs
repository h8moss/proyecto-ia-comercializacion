using System;
using UnityEngine;

public class Objective : MonoBehaviour
{
    public string title;

    public Action<Objective> onCompleted;
    public Action onBecomeActive;
    private bool isActiveObjective = false;
    public bool IsActiveObjective { get => isActiveObjective; }

    public void CompleteObjective()
    {
        isActiveObjective = false;
        onCompleted?.Invoke(this);
    }
    
    public void RaiseOnBecomeActive()
    {
        isActiveObjective = true;
        onBecomeActive?.Invoke();
    }
}
