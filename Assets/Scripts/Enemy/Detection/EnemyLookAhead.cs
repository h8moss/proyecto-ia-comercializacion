using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyLookAhead : MonoBehaviour
{
    Rigidbody2D rb;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 forward = rb.linearVelocity.normalized;
        transform.rotation = Quaternion.LookRotation(forward, Vector3.forward);
    }
}
