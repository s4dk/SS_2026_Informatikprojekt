using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;

public class VRSplineDrawer : MonoBehaviour
{
    [Header("XR References")]
    [Tooltip("Assign the tip of your right-hand controller model here.")]
    [SerializeField] private Transform controllerTip;

    [Header("Input Actions")]
    [SerializeField] private InputActionReference drawAction;   // e.g. Right Trigger
    [SerializeField] private InputActionReference clearAction;  // e.g. Right Primary Button (A)

    [Header("Drawing Settings")]
    [SerializeField] private float minPointDistance = 0.05f;

    [Header("Visualizer Prefab")]
    [Tooltip("Prefab with a SplineVisualizer component on it.")]
    [SerializeField] private SplineVisualizer visualizerPrefab;

    private SplineContainer _activeContainer;
    private SplineVisualizer _activeVisualizer;
    private bool _isDrawing;

    // ── lifecycle ────────────────────────────────────────────────────────────

    private void OnEnable()
    {
        drawAction.action.Enable();
        clearAction.action.Enable();

        drawAction.action.started  += OnDrawStart;
        drawAction.action.canceled += OnDrawEnd;
        clearAction.action.performed += _ => SplineManager.Instance.ClearAll();
    }

    private void OnDisable()
    {
        drawAction.action.started  -= OnDrawStart;
        drawAction.action.canceled -= OnDrawEnd;
        drawAction.action.Disable();
        clearAction.action.Disable();
    }

    private void Update()
    {
        if (!_isDrawing || controllerTip == null) return;

        Vector3 tip = controllerTip.position;

        bool firstPoint = _activeContainer.Spline.Count == 0;
        bool farEnough  = !firstPoint && Vector3.Distance(
            (Vector3)(UnityEngine.Vector3)_activeContainer.Spline[^1].Position, tip
        ) > minPointDistance;

        if (firstPoint || farEnough)
            SplineManager.Instance.AddPoint(_activeContainer, tip);
    }

    // ── input callbacks ──────────────────────────────────────────────────────

    private void OnDrawStart(InputAction.CallbackContext _)
    {
        _activeContainer  = SplineManager.Instance.CreateSpline();
        _activeVisualizer = Instantiate(visualizerPrefab, _activeContainer.transform);
        _activeVisualizer.Attach(_activeContainer);
        _isDrawing = true;
    }

    private void OnDrawEnd(InputAction.CallbackContext _)
    {
        _isDrawing = false;
        _activeVisualizer?.Refresh(); // final clean pass
    }
}