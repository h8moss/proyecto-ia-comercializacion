using UnityEngine;

public class PlayerLocator : MonoBehaviour
{
    static public Transform Player { get; private set;}

    void Awake()
    {
        if (Player == null) Player = transform;
    }

    static public bool IsPlayer(Transform t)
    {
        return t == Player;
    }
}
