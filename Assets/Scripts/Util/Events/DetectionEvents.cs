using System;
using UnityEngine;

public class DetectionEvents
{
    public static event Action OnPlayerDetected;
    public static event Action OnPlayerHidden;
    
    public static void RaisePlayerDetected() => OnPlayerDetected?.Invoke();
    public static void RaisePlayerHidden() => OnPlayerHidden?.Invoke();

}
