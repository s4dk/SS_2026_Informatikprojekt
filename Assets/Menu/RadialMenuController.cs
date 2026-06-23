using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Minimal radial/pie menu: one segment per ToolDefinition laid out in a
/// circle, hovered segment determined from the controller's pointer angle
/// relative to the menu's center, selection confirmed on button release.
/// This is a functional starting point, not styled UI - swap the segment
/// visuals for your own art, or replace GetHoveredSegmentIndex with
/// XRI's UI raycasting if you'd rather drive it through Graphic Raycaster.
/// </summary>
public class RadialMenuController : MonoBehaviour
{
    [SerializeField] private List<ToolDefinition> tools;
    [SerializeField] private RectTransform[] segmentVisuals; // one per tool, pre-placed in a circle in the editor
    [SerializeField] private Transform pointerSource;        // the controller/ray transform used to aim
    [SerializeField] private InputActionReference menuButton; // press to open, hold to aim, release to confirm
    [SerializeField] private GameObject menuRoot;

    private int _hoveredIndex = -1;

    private void OnEnable()
    {
        if (menuButton != null)
        {
            menuButton.action.started += HandleMenuButtonPressed;
            menuButton.action.canceled += ConfirmSelection;
            menuButton.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (menuButton != null)
        {
            menuButton.action.started -= HandleMenuButtonPressed;
            menuButton.action.canceled -= ConfirmSelection;
        }
    }

    private void HandleMenuButtonPressed(InputAction.CallbackContext ctx) => OpenMenu();

    public void OpenMenu()
    {
        menuRoot.SetActive(true);
        // Position/orient menuRoot in front of the controller here if it's spawned on demand
        // rather than being a fixed wrist/world element.
    }

    public void CloseMenu()
    {
        menuRoot.SetActive(false);
        _hoveredIndex = -1;
    }

    private void Update()
    {
        if (!menuRoot.activeSelf) return;
        _hoveredIndex = GetHoveredSegmentIndex();
        UpdateHighlight();
    }

    private int GetHoveredSegmentIndex()
    {
        Vector3 local = transform.InverseTransformPoint(pointerSource.position + pointerSource.forward * 1f);
        float angle = Mathf.Atan2(local.x, local.y) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360f;

        float segmentSize = 360f / tools.Count;
        return Mathf.FloorToInt(angle / segmentSize);
    }

    private void UpdateHighlight()
    {
        for (int i = 0; i < segmentVisuals.Length; i++)
        {
            segmentVisuals[i].localScale = (i == _hoveredIndex) ? Vector3.one * 1.1f : Vector3.one;
        }
    }

    private void ConfirmSelection(InputAction.CallbackContext ctx)
    {
        if (!menuRoot.activeSelf || _hoveredIndex < 0 || _hoveredIndex >= tools.Count) return;

        VRToolManager.Instance.SetActiveTool(tools[_hoveredIndex].toolId);
        CloseMenu();
    }
}