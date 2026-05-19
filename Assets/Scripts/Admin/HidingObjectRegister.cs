using System.Collections.Generic;
using UnityEngine;

public class HidingObjectRegister : MonoBehaviour
{
    public static HidingObjectRegister Instance{get; private set;}
    
    public List<Transform> HidingObjects{get; private set;}

    void Awake()
    {
        HidingObjects = new();
        Instance = this;
    }

    public void RegisterHidingObject(Transform obj)
    {
        HidingObjects.Add(obj);
    }
}
