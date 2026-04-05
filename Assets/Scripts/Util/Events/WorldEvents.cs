using System;
using UnityEngine;

public class WorldEvents : MonoBehaviour
{
    public static event Action<Vector2, float> OnSoundMade;
    
    public static void RaiseSoundMade(Vector2 position, float loudness) => OnSoundMade?.Invoke(position, loudness);
}
