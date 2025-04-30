using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MinimapMode
{
    Mini, Fullscreen
}
public class MinimapController : MonoBehaviour
{
    public static MinimapController Instance;

    [SerializeField] private Vector2 worldSize = new Vector2(400, 400);

    [SerializeField]
    Vector2 fullScreenDimensions = new Vector2(1000, 1000);

    [SerializeField]
    float zoomSpeed = 0.1f;

    [SerializeField]
    float maxZoom = 10f;

    [SerializeField]
    float minZoom = 1f;

    [SerializeField]
    RectTransform scrollViewRectTransform;

    [SerializeField]
    RectTransform contentRectTransform;

    [SerializeField]
    MinimapIcon minimapIconPrefab;

    Matrix4x4 transformationMatrix;

    [SerializeField] private Vector2 worldOriginOffset;

    private MinimapMode currentMiniMapMode = MinimapMode.Mini;
    private MinimapIcon followIcon;
    private Vector2 scrollViewDefaultSize;
    private Vector2 scrollViewDefaultPosition;
    Dictionary<MinimapWorldObject, MinimapIcon> miniMapWorldObjectsLookup = new Dictionary<MinimapWorldObject, MinimapIcon>();
    private void Awake()
    {
        Instance = this;
        scrollViewDefaultSize = scrollViewRectTransform.sizeDelta;
        scrollViewDefaultPosition = scrollViewRectTransform.anchoredPosition;
    }

    private IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        CalculateTransformationMatrix();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            SetMinimapMode(currentMiniMapMode == MinimapMode.Mini ? MinimapMode.Fullscreen : MinimapMode.Mini);
        }

        float zoom = Input.GetAxis("Mouse ScrollWheel");
        ZoomMap(zoom);
        UpdateMiniMapIcons();
        CenterMapOnIcon();
    }

    public void RegisterMinimapWorldObject(MinimapWorldObject miniMapWorldObject, bool followObject = false)
    {
        var minimapIcon = Instantiate(minimapIconPrefab);
        minimapIcon.transform.SetParent(contentRectTransform);
        minimapIcon.transform.SetParent(contentRectTransform);
        minimapIcon.Image.sprite = miniMapWorldObject.MinimapIcon;
        miniMapWorldObjectsLookup[miniMapWorldObject] = minimapIcon;

        if (followObject)
            followIcon = minimapIcon;
    }

    public void RemoveMinimapWorldObject(MinimapWorldObject minimapWorldObject)
    {
        if (miniMapWorldObjectsLookup.TryGetValue(minimapWorldObject, out MinimapIcon icon))
        {
            miniMapWorldObjectsLookup.Remove(minimapWorldObject);
            Destroy(icon.gameObject);
        }
    }


    private Vector2 halfVector2 = new Vector2(0.5f, 0.5f);
    public void SetMinimapMode(MinimapMode mode)
    {
        const float defaultScaleWhenFullScreen = 1.3f; // 1.3f looks good here but it could be anything

        if (mode == currentMiniMapMode)
            return;

        switch (mode)
        {
            case MinimapMode.Mini:
                scrollViewRectTransform.sizeDelta = scrollViewDefaultSize;
                scrollViewRectTransform.anchorMin = Vector2.one;
                scrollViewRectTransform.anchorMax = Vector2.one;
                scrollViewRectTransform.pivot = Vector2.one;
                scrollViewRectTransform.anchoredPosition = scrollViewDefaultPosition;
                currentMiniMapMode = MinimapMode.Mini;
                break;
            case MinimapMode.Fullscreen:
                scrollViewRectTransform.sizeDelta = fullScreenDimensions;
                scrollViewRectTransform.anchorMin = halfVector2;
                scrollViewRectTransform.anchorMax = halfVector2;
                scrollViewRectTransform.pivot = halfVector2;
                scrollViewRectTransform.anchoredPosition = Vector2.zero;
                currentMiniMapMode = MinimapMode.Fullscreen;
                contentRectTransform.transform.localScale = Vector3.one * defaultScaleWhenFullScreen;
                break;
        }
    }

    private void ZoomMap(float zoom)
    {
        if (zoom == 0)
            return;

        float currentMapScale = contentRectTransform.localScale.x;
        float zoomAmount = (zoom > 0 ? zoomSpeed : -zoomSpeed) * currentMapScale;
        float newScale = currentMapScale + zoomAmount;
        float clampedScale = Mathf.Clamp(newScale, minZoom, maxZoom);
        contentRectTransform.localScale = Vector3.one * clampedScale;
    }

    private void CenterMapOnIcon()
    {
        if (followIcon != null)
        {
            float mapScale = contentRectTransform.transform.localScale.x;
            
            contentRectTransform.anchoredPosition = (-followIcon.RectTransform.anchoredPosition * mapScale);
        }
    }

    private void UpdateMiniMapIcons()
    {
        float iconScale = 1 / contentRectTransform.transform.localScale.x;
        foreach (var kvp in miniMapWorldObjectsLookup)
        {
            var miniMapWorldObject = kvp.Key;
            var miniMapIcon = kvp.Value;
            var mapPosition = WorldPositionToMapPosition(miniMapWorldObject.transform.position);

            miniMapIcon.RectTransform.anchoredPosition = mapPosition;
            var rotation = miniMapWorldObject.transform.rotation.eulerAngles;
            miniMapIcon.IconRectTransform.localRotation = Quaternion.AngleAxis(-rotation.y, Vector3.forward);
            miniMapIcon.IconRectTransform.localScale = Vector3.one * iconScale;
        }


    }

    private Vector2 WorldPositionToMapPosition(Vector3 worldPos)
    {
        Vector2 minimapSize = contentRectTransform.rect.size;

        float normalizedX = Mathf.Clamp01((worldPos.x - worldOriginOffset.x) / worldSize.x);
        float normalizedY = Mathf.Clamp01((worldPos.z - worldOriginOffset.y) / worldSize.y);

        float mapPosX = (normalizedX - 0.5f) * minimapSize.x;
        float mapPosY = (normalizedY - 0.5f) * minimapSize.y;

        return new Vector2(mapPosX, mapPosY);
    }


    private void CalculateTransformationMatrix()
    {
        var minimapSize = contentRectTransform.rect.size;
        var worldSize = new Vector2(this.worldSize.x, this.worldSize.y);

        var translation = -minimapSize / 2;
        var scaleRatio = minimapSize / worldSize;

        transformationMatrix = Matrix4x4.TRS(translation, Quaternion.identity, scaleRatio);

        //  {scaleRatio.x,   0,           0,   translation.x},
        //  {  0,        scaleRatio.y,    0,   translation.y},
        //  {  0,            0,           1,            0},
        //  {  0,            0,           0,            1}
    }
}