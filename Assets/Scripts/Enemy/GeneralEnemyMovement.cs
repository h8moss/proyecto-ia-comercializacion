using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class GeneralEnemyMovement : MonoBehaviour
{

    public Action OnFinishedPath;

    [SerializeField] private float speed;

    private List<Vector2> path;
    private int pathIndex;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (HasPath())
        {
            if (pathIndex >= path.Count) {
                OnFinishedPath?.Invoke();
                // UnsetPath();
                return;
            }
            Vector2 target = path[pathIndex];
            Vector2 direction = (target - (Vector2)transform.position).normalized;
            rb.linearVelocity = speed * Time.fixedDeltaTime * direction;

            if (Vector2.Distance(transform.position, target) < 0.1f) pathIndex++;
        } else 
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    public void SetPath(List<Vector2> newPath)
    {
        if (newPath.Count == 0) return;
        path = newPath;
        pathIndex = 0;
        Debug.Log("New path"); 
    }

    public void UnsetPath()
    {
        path = null;
    }

    public bool HasPath()
    {
        return path != null;
    }

    void OnDrawGizmos()
    {
        if (HasPath() && path.Count >= 2)
        {
            Gizmos.DrawLine(path[0], path[path.Count-1]);
        }
    }
}