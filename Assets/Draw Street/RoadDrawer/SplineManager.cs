using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using Vector3 = UnityEngine.Vector3;
using Unity.Mathematics;
using System.Numerics;

public class SplineManager : MonoBehaviour
{
    public static SplineManager Instance { get; private set; }

    private readonly List<SplineContainer> _allSplines = new();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    /// <summary>Creates a new SplineContainer in the scene and returns it.</summary>
    public SplineContainer CreateSpline(string label = "DrawnSpline")
    {
        var go = new GameObject(label);
        var container = go.AddComponent<SplineContainer>();
        container.Spline.Clear();
        _allSplines.Add(container);
        return container;
    }

    /// <summary>Adds a world-space point to the given spline.</summary>
    public void AddPoint(SplineContainer container, Vector3 worldPos)
    {   
        worldPos.y += 0.005f;
        float3 local = container.transform.InverseTransformPoint(worldPos);
        container.Spline.Add(new BezierKnot(local), TangentMode.AutoSmooth);
    }

    /// <summary>Deletes all splines in the scene.</summary>
    public void ClearAll()
    {
        foreach (var s in _allSplines)
            if (s != null) Destroy(s.gameObject);
        _allSplines.Clear();
    }

    public IReadOnlyList<SplineContainer> AllSplines => _allSplines;
}