using UnityEngine;

[RequireComponent(typeof(Interactable))]
public class TeleportInteraction : MonoBehaviour
{
    [SerializeField] private Transform teleportPosition;

    private Interactable interactable;
    
    void Start()
    {
        interactable = GetComponent<Interactable>();
        interactable.onInteraction += Teleport;
    }

    void Teleport()
    {
        var cam = Camera.main.transform;
        var player = PlayerLocator.Player.transform;
        Vector3 cameraPlayerDistance = cam.position - player.position;
        Vector3 newCamPos = teleportPosition.position + cameraPlayerDistance;

        player.position = teleportPosition.position;
        cam.position = newCamPos;
    }
}
