using System;
using UnityEngine;

public class HidingBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject sprite;
    private Rigidbody2D rb;
    private CircleCollider2D coll;
    private PlayerVisual vis;
    public Action OnHidden;
    public Action OnUnhidden;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<CircleCollider2D>();
        vis = GetComponent<PlayerVisual>();
    }

    public void Hide()
    {
        OnHidden?.Invoke();
        coll.enabled = false;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.simulated = true;
        vis.enabled = false;
        sprite.SetActive(false);
    }
    public void Unhide()
    {
        OnUnhidden?.Invoke();
        coll.enabled = true;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.simulated = true;
        vis.enabled = true;
        sprite.SetActive(true);
    }
}
