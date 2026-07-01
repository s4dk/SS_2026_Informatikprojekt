using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;

public class RoadObjectSpawner : MonoBehaviour
{
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private GameObject[] objectsToSpawn;
    [SerializeField] private float spacing = 10f;
    [SerializeField] private float sideOffset = 5f;
    [SerializeField] private bool spawnLeft = true;
    [SerializeField] private bool spawnRight = true;

    [Header("Overlap Prevention")]
    [SerializeField] private LayerMask obstacleLayer; // layer(s) your spawned prefabs live on
    [SerializeField] private float overlapPadding = 0.2f; // small buffer so objects aren't touching edge-to-edge

    private SplineContainer _splineContainer;

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

    private void TrySpawnAt(Vector3 position, Vector3 facing)
    {
        if (Physics.Raycast(position + Vector3.up * 10f, Vector3.down, out RaycastHit hit, 50f, groundLayer))
        {
            position = hit.point;
        }

        GameObject prefab = objectsToSpawn[UnityEngine.Random.Range(1, objectsToSpawn.Length)];
        Quaternion rotation = Quaternion.LookRotation(facing);

        if (IsOverlapping(prefab, position, rotation))
            return; // something's already there, skip this point

        SpawnAt(prefab, position, rotation);
    }

    private bool IsOverlapping(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        Bounds bounds = GetPrefabBounds(prefab);
        Vector3 halfExtents = bounds.extents + Vector3.one * overlapPadding;

        // OverlapBox checks the actual footprint of the object, oriented to its spawn rotation
        Collider[] hits = Physics.OverlapBox(position + bounds.center, halfExtents, rotation, obstacleLayer);
        return hits.Length > 0;
    }

    private Bounds GetPrefabBounds(GameObject prefab)
    {
        Renderer r = prefab.GetComponentInChildren<Renderer>();
        if (r != null) return r.bounds;

        Collider c = prefab.GetComponentInChildren<Collider>();
        if (c != null) return c.bounds;

        return new Bounds(Vector3.zero, Vector3.one * 0.5f); // fallback
    }

    private void SpawnAt(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        GameObject spawner = objectsToSpawn[0];
        Instantiate(prefab, position, rotation);
        Instantiate(spawner, position, rotation);
    }
}