using System.Runtime.CompilerServices;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    [SerializeField] private Vector3 offset;
    [SerializeField] private float lerp;
    private Transform player;
    void Start()
    {
        player = PlayerLocator.Player;
    }

    void LateUpdate()
    {
        transform.position = Vector3.Lerp(
            transform.position,
            player.position + offset,
            lerp * Time.deltaTime
        );
    }
}
