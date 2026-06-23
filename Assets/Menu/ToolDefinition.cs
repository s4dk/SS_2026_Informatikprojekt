using UnityEngine;

/// <summary>
/// Data-only description of a tool for the radial menu to display.
/// Create one asset per tool (Create > VR Tools > Tool Definition)
/// and assign it both to the menu's tool list and to the matching
/// MonoBehaviour that implements IVRTool with the same toolId.
/// </summary>
[CreateAssetMenu(menuName = "VR Tools/Tool Definition", fileName = "NewToolDefinition")]
public class ToolDefinition : ScriptableObject
{
    [Tooltip("Must match the ToolId returned by the corresponding IVRTool implementation.")]
    public string toolId;
    public string displayName;
    public Sprite icon;
}
