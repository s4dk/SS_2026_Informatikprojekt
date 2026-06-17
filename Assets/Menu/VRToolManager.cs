using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Central authority for which VR tool is currently active and for
/// routing the controller's action input to that tool only.
/// Attach this once to a persistent manager object in the scene.
/// </summary>
public class VRToolManager : MonoBehaviour
{
    public static VRToolManager Instance { get; private set; }

    [Tooltip("The XRI Input Action used for the tool's action button (trigger, etc).")]
    [SerializeField] private InputActionReference actionButton;

    private readonly Dictionary<string, IVRTool> _registeredTools = new();
    private IVRTool _activeTool;

    public string ActiveToolId => _activeTool?.ToolId;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnEnable()
    {
        if (actionButton != null)
        {
            actionButton.action.started += HandlePressed;
            actionButton.action.canceled += HandleReleased;
            actionButton.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (actionButton != null)
        {
            actionButton.action.started -= HandlePressed;
            actionButton.action.canceled -= HandleReleased;
        }
    }

    private void Update()
    {
        if (_activeTool != null && actionButton != null && actionButton.action.IsPressed())
        {
            _activeTool.OnActionHeld();
        }
    }

    private void HandlePressed(InputAction.CallbackContext ctx) => _activeTool?.OnActionPressed();
    private void HandleReleased(InputAction.CallbackContext ctx) => _activeTool?.OnActionReleased();

    /// Call this from each tool's Awake/Start so the manager knows it exists.
    public void RegisterTool(IVRTool tool)
    {
        _registeredTools[tool.ToolId] = tool;
    }

    public void UnregisterTool(IVRTool tool)
    {
        if (_registeredTools.TryGetValue(tool.ToolId, out var existing) && existing == tool)
        {
            _registeredTools.Remove(tool.ToolId);
        }
    }

    /// Call this from the radial menu when the user confirms a slice.
    public void SetActiveTool(string toolId)
    {
        if (_activeTool != null && _activeTool.ToolId == toolId) return;

        if (!_registeredTools.TryGetValue(toolId, out var nextTool))
        {
            Debug.LogWarning($"VRToolManager: no tool registered with id '{toolId}'.");
            return;
        }

        _activeTool?.OnToolDeactivated();
        _activeTool = nextTool;
        _activeTool.OnToolActivated();
    }

    /// Optional: call this if you want an explicit "no tool selected" idle state.
    public void ClearActiveTool()
    {
        _activeTool?.OnToolDeactivated();
        _activeTool = null;
    }
}
