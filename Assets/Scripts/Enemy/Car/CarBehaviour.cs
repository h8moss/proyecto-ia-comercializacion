using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

/// <summary>
/// Drives a 2D car along A* paths on the street graph.
///
/// SETUP:
///   1. Add this component to your car prefab — Rigidbody2D, AIPath and Seeker
///      are required and enforced by RequireComponent below.
///   2. In the AIPath Inspector: set "Orientation" to "YAxisForward" (top-down).
///      Tick "Can Move" OFF — this script drives movement, not AIPath.
///   3. Make sure your A* graph is baked to street-only geometry.
///   4. Assign despawnPoints in the Inspector, or let CarSpawner do it.
///   5. Create a "Car" layer, assign it to the prefab, set carLayerMask to it.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AIPath))]
[RequireComponent(typeof(Seeker))]
public class CarBehaviour : MonoBehaviour
{
    // -------------------------------------------------------------------------
    // Inspector fields
    // -------------------------------------------------------------------------

    [Header("Movement")]
    [Tooltip("Normal cruising speed in units/s.")]
    public float targetSpeed = 8f;

    [Tooltip("How quickly the car accelerates and brakes (units/s²).")]
    public float acceleration = 4f;

    [Tooltip("How quickly the car rotates to face its direction of travel (degrees/s).")]
    public float turnSpeed = 200f;

    [Header("Avoidance")]
    [Tooltip("How far ahead to look for cars in the way.")]
    public float brakingDistance = 6f;

    [Tooltip("Half-width of the avoidance CircleCast — roughly half the car's width.")]
    public float brakingWidth = 0.8f;

    [Tooltip("Layer mask for other cars.")]
    public LayerMask carLayerMask;

    [Header("Waypoints")]
    [Tooltip("How close the car must be to a waypoint before advancing to the next one.")]
    public float waypointReachedDistance = 1.2f;

    [Tooltip("Possible despawn points at the edges of the map. Assign in Inspector.")]
    public Transform[] despawnPoints;

    // -------------------------------------------------------------------------
    // Private state
    // -------------------------------------------------------------------------

    private Rigidbody2D rb;
    private AIPath aiPath;
    private Seeker seeker;

    private List<Vector2> waypoints = new List<Vector2>();
    private int waypointIndex = 0;
    private float currentSpeed = 0f;
    private bool reachedDestination = false;

    // -------------------------------------------------------------------------
    // Unity lifecycle
    // -------------------------------------------------------------------------

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        aiPath = GetComponent<AIPath>();
        seeker = GetComponent<Seeker>();

        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        // We control movement in FixedUpdate — prevent AIPath from also moving the object.
        aiPath.canMove = false;
    }

    void Start()
    {
        // Small random variation so cars don't all travel at the same pace.
        targetSpeed += Random.Range(-1.5f, 1.5f);

        RequestNewPath();
    }

    void FixedUpdate()
    {
        if (reachedDestination || waypoints.Count == 0) return;

        Vector2 target = waypoints[waypointIndex];
        Vector2 position2D = rb.position;

        // --- Avoidance: slow down if a car is directly ahead ---
        float speedLimit = GetAvoidanceSpeedLimit();
        currentSpeed = Mathf.MoveTowards(currentSpeed, speedLimit, acceleration * Time.fixedDeltaTime);

        // --- Rotation: face the current target waypoint ---
        Vector2 direction = (target - position2D).normalized;
        if (direction.sqrMagnitude > 0.001f)
        {
            // Atan2 gives the angle in world space; subtract 90° because
            // Unity 2D sprites typically point "up" as their forward direction.
            // float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 0f;
            float newAngle = Mathf.MoveTowardsAngle(rb.rotation, targetAngle, turnSpeed * Time.fixedDeltaTime);
            rb.MoveRotation(newAngle);
        }

        // --- Move along the car's local up axis (its visual forward in 2D) ---
        // Vector2 forward2D = transform.up;
        Vector2 forward2D = transform.right;
        rb.MovePosition(position2D + forward2D * currentSpeed * Time.fixedDeltaTime);

        // --- Advance to the next waypoint when close enough ---
        if (Vector2.Distance(position2D, target) <= waypointReachedDistance)
        {
            waypointIndex++;

            if (waypointIndex >= waypoints.Count)
            {
                reachedDestination = true;
                Destroy(gameObject, 0.3f);
            }
        }
    }

    // -------------------------------------------------------------------------
    // Pathfinding
    // -------------------------------------------------------------------------

    void RequestNewPath()
    {
        if (despawnPoints == null || despawnPoints.Length == 0)
        {
            Debug.LogWarning("CarAI: No despawn points assigned on " + gameObject.name);
            return;
        }

        Transform destination = despawnPoints[Random.Range(0, despawnPoints.Length)];
        seeker.StartPath(transform.position, destination.position, OnPathComplete);
    }

    void OnPathComplete(Path p)
    {
        if (p.error)
        {
            Debug.LogWarning("CarAI: Path error — " + p.errorLog);
            return;
        }

        waypoints.Clear();
        foreach (Vector3 point in p.vectorPath)
            waypoints.Add(new Vector2(point.x, point.y));

        waypointIndex = 0;
        reachedDestination = false;
    }

    // -------------------------------------------------------------------------
    // Avoidance
    // -------------------------------------------------------------------------

    /// <summary>
    /// CircleCasts forward in 2D. Returns a reduced speed when another car is
    /// close ahead, or full targetSpeed when the road ahead is clear.
    /// </summary>
    float GetAvoidanceSpeedLimit()
    {
        Vector2 origin = rb.position + (Vector2)(transform.up * 0.5f);

        RaycastHit2D hit = Physics2D.CircleCast(
            origin,
            brakingWidth,
            transform.up,
            brakingDistance,
            carLayerMask
        );

        if (hit.collider != null && hit.collider.gameObject != gameObject)
        {
            float proximity = 1f - Mathf.Clamp01(hit.distance / brakingDistance);
            return Mathf.Lerp(targetSpeed, 0f, proximity);
        }

        return targetSpeed;
    }

    // -------------------------------------------------------------------------
    // Debug gizmos (visible when the car is selected in the editor)
    // -------------------------------------------------------------------------

    void OnDrawGizmosSelected()
    {
        // Remaining path
        if (waypoints != null && waypoints.Count > 1)
        {
            Gizmos.color = Color.cyan;
            for (int i = Mathf.Max(waypointIndex, 0); i < waypoints.Count - 1; i++)
                Gizmos.DrawLine(waypoints[i], waypoints[i + 1]);

            if (waypointIndex < waypoints.Count)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(waypoints[waypointIndex], 0.3f);
            }
        }

        // Avoidance cast preview
        Gizmos.color = Color.red;
        Vector2 origin = (Vector2)transform.position + (Vector2)(transform.up * 0.5f);
        Gizmos.DrawLine(origin, origin + (Vector2)(transform.up * brakingDistance));
        Gizmos.DrawWireSphere(origin + (Vector2)(transform.up * brakingDistance), brakingWidth);
    }
}