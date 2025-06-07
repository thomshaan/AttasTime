#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class MinimapValidator : MonoBehaviour
{
    public RectTransform contentRect;
    public Vector2 expectedPivot = new Vector2(0.5f, 0.5f);

    void OnEnable()
    {
        Validate();
    }

    [ContextMenu("Validate Minimap Settings")]
    public void Validate()
    {
        if (contentRect == null)
        {
            Debug.LogWarning("[MinimapValidator] Content RectTransform is not assigned.");
            return;
        }

        if (contentRect.pivot != expectedPivot)
        {
            Debug.LogWarning($"[MinimapValidator] Pivot is {contentRect.pivot}, expected {expectedPivot}. Set to center for accurate mapping.");
        }

        if (contentRect.anchorMin != expectedPivot || contentRect.anchorMax != expectedPivot)
        {
            Debug.LogWarning($"[MinimapValidator] Anchors are not centered. Set both Anchor Min and Max to (0.5, 0.5).");
        }

        if (contentRect.anchoredPosition != Vector2.zero)
        {
            Debug.LogWarning($"[MinimapValidator] Anchored position should be zero for centered content.");
        }

        if (contentRect.localScale != Vector3.one)
        {
            Debug.LogWarning($"[MinimapValidator] Content scale is not 1. This may distort icon positions.");
        }

        Debug.Log("[MinimapValidator] Validation complete.");
    }
}
#endif
