using UnityEngine;

public class MinimapWorldObject : MonoBehaviour
{
    [SerializeField]
    private bool followObject = false;
    [SerializeField]
    private Sprite minimapIcon;
    public Sprite MinimapIcon => minimapIcon;

    private void Start()
    {
        MinimapController.Instance.RegisterMinimapWorldObject(this, followObject);
    }

    void OnDestroy()
    {
        if (MinimapController.Instance != null)
        {
            MinimapController.Instance.RemoveMinimapWorldObject(this);
        }
    }
}