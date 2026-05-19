using UnityEngine;

/// <summary>
/// Tiny component required by CarSpawner to track when a car is destroyed.
/// Add this to your car prefab alongside CarAI.
/// </summary>
[RequireComponent(typeof(CarBehaviour))]
public class CarLifetimeTracker : MonoBehaviour
{
    public System.Action OnCarDestroyed;
 
    void OnDestroy()
    {
        OnCarDestroyed?.Invoke();
    }
}
 
