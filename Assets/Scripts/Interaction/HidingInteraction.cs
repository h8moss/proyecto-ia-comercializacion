using UnityEngine;

[RequireComponent(typeof(Interactable))]
public class HidingInteraction : MonoBehaviour
{
    [SerializeField] private Transform leftExitPoint;
    [SerializeField] private Transform rightExitPoint;
    [SerializeField] private Transform topExitPoint;
    [SerializeField] private Transform bottomExitPoint;
    private Interactable interactable;
    private bool isHiding = false;
    private bool canExit = false;

    void Start()
    {
        interactable = GetComponent<Interactable>();
        interactable.onInteraction += OnInteraction;
    }

    void Update()
    {
        if (isHiding)
        {
            Vector2 movement = new(
                Input.GetAxisRaw("Horizontal"),
                Input.GetAxisRaw("Vertical")
            );
            if (movement.magnitude > 0.2)
            {
                if (!canExit) return;
                GameObject player = PlayerLocator.Player.gameObject;
                player.SetActive(true);
                isHiding = false;

                if (movement.x > 0.2) {
                    player.transform.position = rightExitPoint.position;
                } else if (movement.x < -0.2) {
                    player.transform.position = leftExitPoint.position;
                } else if (movement.y > 0.2) {
                    player.transform.position = topExitPoint.position;
                } else {
                    player.transform.position = bottomExitPoint.position;
                }
            } else
            {
                canExit = true;
            }
        }
    }

    void OnInteraction()
    {
        Transform player = PlayerLocator.Player;
        player.gameObject.SetActive(false);
        isHiding = true;
        canExit = false;
    }
}
