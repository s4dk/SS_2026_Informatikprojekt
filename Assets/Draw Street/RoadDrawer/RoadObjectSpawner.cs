using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;
using System.Collections.Generic;

public class RoadObjectSpawner : MonoBehaviour
{
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private GameObject[] objectsToSpawn;
    [SerializeField] private float spacing = 10f;
    [SerializeField] private float sideOffset = 5f;
    [SerializeField] private bool spawnLeft = true;
    [SerializeField] private bool spawnRight = true;

[Header("Overlap Prevention")]
[SerializeField] private LayerMask obstacleLayer;
[SerializeField] private LayerMask roadLayer; // road mesh colliders live here
[SerializeField] private float overlapPadding = 0.2f;

[Header("Spline Collision Clearing")]
[SerializeField] private float roadWidth = 1f; // horizontal clearing radius, no buffer
[SerializeField] private bool ignoreHeightDifference = true; // road width is a horizontal concept

    private SplineContainer _splineContainer;

    // shared across ALL RoadObjectSpawner instances, so any new road can clear objects from any other road
    private static readonly List<GameObject> _allSpawnedObjects = new List<GameObject>();

    public void SetSpline(SplineContainer container)
    {
        _splineContainer = container;
    }

    public void SpawnObjects()
    {
        if (_splineContainer == null)
        {
            Debug.LogWarning("RoadObjectSpawner: No spline assigned.");
            return;
        }

        // clear anything (from this road or any other) sitting on the path before placing new objects
        ClearObjectsOnSpline(_splineContainer, roadWidth, ignoreHeightDifference);

        Spline spline = _splineContainer.Spline;
        float length = spline.GetLength();
        int count = Mathf.FloorToInt(length / spacing);

        for (int i = 0; i < count; i++)
        {
            float t = i / (float)count;
            SplineUtility.Evaluate(spline, t, out float3 pos, out float3 tangent, out float3 upVector);

            Vector3 worldPos = _splineContainer.transform.TransformPoint((Vector3)pos);
            Vector3 forward = Vector3.Normalize((Vector3)tangent);
            Vector3 right = Vector3.Cross(forward, (Vector3)upVector).normalized;

            if (spawnRight)
                TrySpawnAt(worldPos + right * sideOffset, -right);
            if (spawnLeft)
                TrySpawnAt(worldPos - right * sideOffset, right);
        }
    }

    /// <summary>
    /// Destroys any tracked spawned object that lies within roadWidth of the given spline's path.
    /// Works across ALL RoadObjectSpawner instances, not just this one.
    /// </summary>
public static void ClearObjectsOnSpline(SplineContainer container, float roadWidth, bool ignoreHeight = true)
{
    if (container == null) return;
    Spline spline = container.Spline;

    for (int j = _allSpawnedObjects.Count - 1; j >= 0; j--)
    {
        GameObject obj = _allSpawnedObjects[j];
        if (obj == null)
        {
            _allSpawnedObjects.RemoveAt(j); // clean up destroyed refs
            continue;
        }

        // convert object's world position into the spline's local space
        float3 localPos = container.transform.InverseTransformPoint(obj.transform.position);

        // exact nearest point on the curve — not a sampled approximation
        SplineUtility.GetNearestPoint(spline, localPos, out float3 nearest, out float t);
        Vector3 nearestWorld = container.transform.TransformPoint((Vector3)nearest);

        float dist;
        if (ignoreHeight)
        {
            // flatten both points to XZ so hills/slopes don't skew the distance
            Vector3 a = new Vector3(obj.transform.position.x, 0f, obj.transform.position.z);
            Vector3 b = new Vector3(nearestWorld.x, 0f, nearestWorld.z);
            dist = Vector3.Distance(a, b);
        }
        else
        {
            dist = Vector3.Distance(obj.transform.position, nearestWorld);
        }

        if (dist <= roadWidth)
        {
            _allSpawnedObjects.RemoveAt(j);
            Destroy(obj);
        }
    }
}
private void TrySpawnAt(Vector3 position, Vector3 facing)
{
    if (Physics.Raycast(position + Vector3.up * 10f, Vector3.down, out RaycastHit hit, 50f, groundLayer))
    {
        position = hit.point;
    }

    GameObject prefab = objectsToSpawn[UnityEngine.Random.Range(1, objectsToSpawn.Length)];
    Quaternion rotation = Quaternion.LookRotation(facing);

    if (IsOverlapping(prefab, position, rotation, obstacleLayer))
        return; // blocked by another spawned object

    if (IsOverlapping(prefab, position, rotation, roadLayer))
        return; // blocked by a road mesh

    SpawnAt(prefab, position, rotation);
}

private bool IsOverlapping(GameObject prefab, Vector3 position, Quaternion rotation, LayerMask layer)
{
    Bounds bounds = GetPrefabBounds(prefab);
    Vector3 halfExtents = bounds.extents + Vector3.one * overlapPadding;
    Collider[] hits = Physics.OverlapBox(position + bounds.center, halfExtents, rotation, layer);
    return hits.Length > 0;
}
    private Bounds GetPrefabBounds(GameObject prefab)
    {
        Renderer r = prefab.GetComponentInChildren<Renderer>();
        if (r != null) return r.bounds;

        Collider c = prefab.GetComponentInChildren<Collider>();
        if (c != null) return c.bounds;

        return new Bounds(Vector3.zero, Vector3.one * 0.5f);
    }

    private void SpawnAt(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        GameObject spawner = objectsToSpawn[0];
        GameObject spawnedPrefab = Instantiate(prefab, position, rotation);
        GameObject spawnedSpawner = Instantiate(spawner, position, rotation);

        _allSpawnedObjects.Add(spawnedPrefab);
        _allSpawnedObjects.Add(spawnedSpawner);
    }
}