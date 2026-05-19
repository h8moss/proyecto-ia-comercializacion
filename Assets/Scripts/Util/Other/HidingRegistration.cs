using UnityEngine;

public class HidingRegistration : MonoBehaviour
{
    void Start()
    {
        Debug.Log(HidingObjectRegister.Instance);
        HidingObjectRegister.Instance.RegisterHidingObject(transform);
    }

}
