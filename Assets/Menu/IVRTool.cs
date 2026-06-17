using UnityEngine;

/// <summary>
/// Contract every selectable VR tool must implement so the
/// VRToolManager can route activation state and input events to it
/// without knowing the concrete tool type.
/// </summary>
public interface IVRTool
{
    /// Unique id matching the corresponding ToolDefinition.toolId
    string ToolId { get; }

    /// Called once when this tool becomes the active tool.
    void OnToolActivated();

    /// Called once when this tool stops being the active tool.
    /// Use this to cancel/finalize any in-progress action
    /// (e.g. an unfinished spline) so switching tools mid-action is safe.
    void OnToolDeactivated();

    /// Forwarded every frame the action button is held while this tool is active.
    void OnActionHeld();

    /// Forwarded once on the frame the action button is pressed.
    void OnActionPressed();

    /// Forwarded once on the frame the action button is released.
    void OnActionReleased();
}
