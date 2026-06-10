using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;
using Unity.Splines.Examples;


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
    private bool _isDrawing;

    private void Update()
    {
        if (!_isDrawing) return;

        Vector3 point = TryGetRayHitPoint(out Vector3 hitPoint)
            ? hitPoint
            : rayInteractor.transform.position +
              rayInteractor.transform.forward * maxRayDistance;

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
        // Create the spline GameObject
        _activeContainer = SplineManager.Instance.CreateSpline();

        // Add LoafRoadBehaviour to the same GameObject —
        // it will read the SplineContainer automatically
        object value = _activeContainer.gameObject.AddComponent<LoftRoadBehaviour>();
    

        var road = _activeContainer.gameObject.AddComponent<LoftRoadBehaviour>();

       

   

        _isDrawing = true;
    }

    private void OnDrawEnd(InputAction.CallbackContext _)
    {
        _isDrawing = false;
    }

    // ── ray helpers ──────────────────────────────────────────────────────────

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