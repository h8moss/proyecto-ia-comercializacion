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
    public bool IsHiding => isHiding;   // ← getter público, sigue siendo privada por dentro
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

                if (movement.x > 0.2 && rightExitPoint.gameObject.activeInHierarchy) {
                    player.transform.position = rightExitPoint.position;
                } else if (movement.x < -0.2  && leftExitPoint.gameObject.activeInHierarchy) {
                    player.transform.position = leftExitPoint.position;
                } else if (movement.y > 0.2 && topExitPoint.gameObject.activeInHierarchy) {
                    player.transform.position = topExitPoint.position;
                } else if (bottomExitPoint.gameObject.activeInHierarchy) {
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

    /// <summary>
    /// Forza la salida del escondite desde fuera (ej: cuando un enemigo descubre al jugador).
    /// Reactiva al jugador y resetea el estado del escondite.
    /// </summary>
    public void ForceExit()
    {
        if (!isHiding) return;

        GameObject player = PlayerLocator.Player.gameObject;
        player.SetActive(true);
        player.transform.position = transform.position;
        isHiding = false;
        canExit = false;
    }
}