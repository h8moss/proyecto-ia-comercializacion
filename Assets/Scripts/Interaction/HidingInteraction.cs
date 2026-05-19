using UnityEngine;

[RequireComponent(typeof(Interactable))]
public class HidingInteraction : MonoBehaviour
{
    [SerializeField] private Transform leftExitPoint;
    [SerializeField] private Transform rightExitPoint;
    [SerializeField] private Transform topExitPoint;
    [SerializeField] private Transform bottomExitPoint;
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite hidingSprite;
    [SerializeField] private SpriteRenderer sprite;

    private Interactable interactable;
    private bool isHiding = false;
    private bool canExit = false;

    public bool IsHiding => isHiding;   // ← getter público, sigue siendo privada por dentro

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
                player.GetComponent<HidingBehaviour>().Unhide();
                isHiding = false;
                sprite.sprite = normalSprite;

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
        player.GetComponent<HidingBehaviour>().Hide();
        isHiding = true;
        sprite.sprite = hidingSprite;
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
        player.GetComponent<HidingBehaviour>().Unhide();
        if (rightExitPoint.gameObject.activeInHierarchy) {
            player.transform.position = rightExitPoint.position;
        } else if (leftExitPoint.gameObject.activeInHierarchy) {
            player.transform.position = leftExitPoint.position;
        } else if (topExitPoint.gameObject.activeInHierarchy) {
            player.transform.position = topExitPoint.position;
        } else if (bottomExitPoint.gameObject.activeInHierarchy) {
            player.transform.position = bottomExitPoint.position;
        }

        isHiding = false;
        sprite.sprite = normalSprite;
        canExit = false;
    }
}