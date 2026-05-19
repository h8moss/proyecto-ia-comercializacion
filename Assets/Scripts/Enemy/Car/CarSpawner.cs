using System.Collections;
using UnityEngine;

/// <summary>
/// Spawns car prefabs at street-edge points and wires up their despawn targets.
///
/// SETUP:
///   1. Create an empty GameObject in the scene and add this component.
///   2. Assign your car prefab (must have CarAI on it) to carPrefab.
///   3. Assign spawn point Transforms (road entrances) to spawnPoints.
///   4. Assign despawn point Transforms (road exits) to despawnPoints.
///   5. Tune spawnInterval and maxCars to taste.
/// </summary>
public class CarSpawner : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Car prefab with the CarAI component.")]
    public GameObject carPrefab;

    [Tooltip("Road-edge positions where cars appear.")]
    public Transform[] spawnPoints;

    [Tooltip("Road-edge positions where cars drive toward and disappear.")]
    public Transform[] despawnPoints;

    [Header("Tuning")]
    [Tooltip("Seconds between each spawn attempt.")]
    public float spawnInterval = 3f;

    [Tooltip("Maximum number of cars alive at once.")]
    public int maxCars = 10;

    [Tooltip("Radius around a spawn point that must be clear before spawning.")]
    public float spawnClearanceRadius = 3f;

    [Tooltip("Car layer — used to check whether a spawn point is blocked.")]
    public LayerMask carLayerMask;

    // -------------------------------------------------------------------------

    private int activeCars = 0;

    void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (activeCars < maxCars)
                TrySpawn();
        }
    }

    void TrySpawn()
    {
        if (carPrefab == null || spawnPoints == null || spawnPoints.Length == 0) return;

        Transform spawnPoint = GetClearSpawnPoint();
        if (spawnPoint == null) return;

        GameObject car = Instantiate(carPrefab, spawnPoint.position, spawnPoint.rotation);

        CarBehaviour ai = car.GetComponent<CarBehaviour>();
        if (ai != null)
            ai.despawnPoints = despawnPoints;

        activeCars++;

        // Decrement the counter automatically when the car is destroyed.
        CarLifetimeTracker tracker = car.GetComponent<CarLifetimeTracker>();
        tracker.OnCarDestroyed += () => activeCars--;
    }

    /// <summary>
    /// Returns a randomly chosen spawn point with no car overlapping it,
    /// or null if every point is currently blocked.
    /// </summary>
    Transform GetClearSpawnPoint()
    {
        int[] indices = ShuffledIndices(spawnPoints.Length);

        foreach (int i in indices)
        {
            Transform point = spawnPoints[i];
            Collider2D hit = Physics2D.OverlapCircle(point.position, spawnClearanceRadius, carLayerMask);
            if (hit == null)
                return point;
        }

        return null;
    }

    int[] ShuffledIndices(int count)
    {
        int[] arr = new int[count];
        for (int i = 0; i < count; i++) arr[i] = i;
        for (int i = count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (arr[i], arr[j]) = (arr[j], arr[i]);
        }
        return arr;
    }

    void OnDrawGizmos()
    {
        if (spawnPoints != null)
        {
            Gizmos.color = Color.green;
            foreach (var p in spawnPoints)
                if (p != null) Gizmos.DrawWireSphere(p.position, 0.6f);
        }

        if (despawnPoints != null)
        {
            Gizmos.color = Color.red;
            foreach (var p in despawnPoints)
                if (p != null) Gizmos.DrawWireSphere(p.position, 0.6f);
        }
    }
}

