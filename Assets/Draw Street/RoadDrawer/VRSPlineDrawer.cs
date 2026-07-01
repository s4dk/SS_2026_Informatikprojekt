using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;
using Unity.Splines.Examples;
using Unity.Mathematics;

public class VRSplineDrawer : MonoBehaviour
{
    [Header("XR References")]
    [SerializeField] private UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor rayInteractor;
    [Header("Input Actions")]
    [SerializeField] private InputActionReference drawAction;
    [SerializeField] private InputActionReference clearAction;
    [Header("Drawing Settings")]
    [SerializeField] private float minPointDistance = 0.05f;
    [SerializeField] private float maxRayDistance = 10f;

    // ── new: table layer mask ─────────────────────────────────────────────────
    private readonly LayerMask _tableLayer = ~0; // set in Awake
    private int _tableLayerIndex;

    private void Awake()
    {
        _tableLayerIndex = LayerMask.NameToLayer("table");
        if (_tableLayerIndex == -1)
            Debug.LogWarning("VRSplineDrawer: 'table' layer not found. Create it in Project Settings → Tags and Layers.");
    }

    // ── lifecycle ────────────────────────────────────────────────────────────
    private void OnEnable()
    {
        drawAction.action.Enable();
        clearAction.action.Enable();
        drawAction.action.started    += OnDrawStart;
        drawAction.action.canceled   += OnDrawEnd;
        clearAction.action.performed += _ => SplineManager.Instance.ClearAll();
    }

    private void OnDisable()
    {
        drawAction.action.started    -= OnDrawStart;
        drawAction.action.canceled   -= OnDrawEnd;
        drawAction.action.Disable();
        clearAction.action.Disable();
    }

private SplineContainer _activeContainer;
private LoftRoadBehaviour _activeRoad;        
private bool _isDrawing;

[SerializeField] private RoadObjectSpawner spawner;  

    private void Update()
    {
        if (!_isDrawing) return;

        // Only add points if the ray is hitting the table
        if (!TryGetTableHitPoint(out Vector3 point)) return;

        bool firstPoint = _activeContainer.Spline.Count == 0;
        bool farEnough  = !firstPoint && Vector3.Distance(
            (Vector3)_activeContainer.Spline[^1].Position, point
        ) > minPointDistance;

        if (firstPoint || farEnough)
            SplineManager.Instance.AddPoint(_activeContainer, point);
    }

    // ── input callbacks ──────────────────────────────────────────────────────

private void OnDrawStart(InputAction.CallbackContext _)
{
    _activeContainer = SplineManager.Instance.CreateSpline();
    _activeRoad = _activeContainer.gameObject.AddComponent<LoftRoadBehaviour>();

    _isDrawing = true;
}

private void OnDrawEnd(InputAction.CallbackContext _)
{
    _isDrawing = false;

    if (spawner != null && _activeContainer != null)
    {
        spawner.SetSpline(_activeContainer);
        spawner.SpawnObjects();
    }

    SplineRegistry.Instance.RegisterSpline(_activeContainer);

    _activeContainer = null;
    _activeRoad = null;
}

    // ── ray helpers ──────────────────────────────────────────────────────────

    // Only returns true if the hit object is on the "table" layer
    private bool TryGetTableHitPoint(out Vector3 hitPoint)
    {
        hitPoint = Vector3.zero;

        if (rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
        {
            if (hit.collider.gameObject.layer == _tableLayerIndex)
            {
                hitPoint = hit.point;
                return true;
            }
        }

        return false;
    }

    // Kept for any future use where you want hits on any surface
    private bool TryGetRayHitPoint(out Vector3 hitPoint)
    {
        hitPoint = Vector3.zero;
        if (rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
        {
            hitPoint = hit.point;
            return true;
        }
        if (rayInteractor.TryGetCurrentUIRaycastResult(
                out UnityEngine.EventSystems.RaycastResult uiHit))
        {
            hitPoint = uiHit.worldPosition;
            return true;
        }
        return false;
    }

}