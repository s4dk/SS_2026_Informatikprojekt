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

    private SplineContainer _splineContainer;  // ← added

    public void SetSpline(SplineContainer container)  // ← added
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
                SpawnAt(worldPos + right * sideOffset, forward);

            if (spawnLeft)
                SpawnAt(worldPos - right * sideOffset, forward);
        }
    }

    private void SpawnAt(Vector3 position, Vector3 forward)
    {
        if (Physics.Raycast(position + Vector3.up * 10f, Vector3.down, out RaycastHit hit, 50f, groundLayer))
        {
            position = hit.point;
        }

        GameObject prefab = objectsToSpawn[UnityEngine.Random.Range(0, objectsToSpawn.Length)];
        Instantiate(prefab, position, Quaternion.LookRotation(forward));
    }
}