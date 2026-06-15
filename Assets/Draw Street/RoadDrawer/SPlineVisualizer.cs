using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(LineRenderer))]
public class SplineVisualizer : MonoBehaviour
{
    [Tooltip("Number of sample points along the spline curve.")]
    [SerializeField] private int resolution = 80;
    [SerializeField] private float lineWidth = 0.008f;
    [SerializeField] private Material lineMaterial;   // assign in Inspector

    private LineRenderer _lr;
    private SplineContainer _container;

    private void Awake()
    {
        _lr = GetComponent<LineRenderer>();
        _lr.startWidth = lineWidth;
        _lr.endWidth   = lineWidth;
        _lr.useWorldSpace = true;
        if (lineMaterial) _lr.material = lineMaterial;
    }

    /// <summary>Attach to a SplineContainer to start live rendering.</summary>
    public void Attach(SplineContainer container) => _container = container;

    private void Update()
    {
        if (_container == null || _container.Spline.Count < 2) return;
        Refresh();
    }

    public void Refresh()
    {
        if (_container == null) return;
        _lr.positionCount = resolution;
        for (int i = 0; i < resolution; i++)
        {
            float t = i / (float)(resolution - 1);
            _lr.SetPosition(i, _container.EvaluatePosition(t));
        }
    }
}