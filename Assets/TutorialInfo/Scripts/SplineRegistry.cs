using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class SplineRegistry : MonoBehaviour
{
    public static SplineRegistry Instance { get; private set; }

    private readonly List<SplineContainer> allSplines = new();

    public static event Action<SplineContainer> OnSplineCreated;

    private void Awake()
    {
        Instance = this;
    }

    public void RegisterSpline(SplineContainer spline)
    {
        allSplines.Add(spline);
        OnSplineCreated?.Invoke(spline);
    }

    public void UnregisterSpline(SplineContainer spline)
    {
        allSplines.Remove(spline);
    }

    public IReadOnlyList<SplineContainer> AllSplines => allSplines;
}