using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollowPlayer : MonoBehaviour
{
    [SerializeField] private Vector3 offset;
    [SerializeField] private float lerp;

    [Header("Camera Bounds")]
    [SerializeField] private Vector2 minBounds;
    [SerializeField] private Vector2 maxBounds;

    private Transform player;
    private Camera cam;

    void Start()
    {
        player = PlayerLocator.Player;
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        Vector3 target = player.position + offset;

        // Lerp toward the target as before
        Vector3 newPos = Vector3.Lerp(transform.position, target, lerp * Time.deltaTime);

        // Clamp so the camera view stays within the map bounds
        float camHalfHeight = cam.orthographicSize;
        float camHalfWidth  = cam.orthographicSize * cam.aspect;

        newPos.x = Mathf.Clamp(newPos.x, minBounds.x + camHalfWidth,  maxBounds.x - camHalfWidth);
        newPos.y = Mathf.Clamp(newPos.y, minBounds.y + camHalfHeight, maxBounds.y - camHalfHeight);

        transform.position = newPos;
    }
}
