using UnityEngine;
using UnityEngine.UI;

public class FloatingIconController : MonoBehaviour
{
    private GameObject iconUIInstance;
    private Image iconImage;
    private Transform target;
    public Vector3 offset = new Vector3(0, 2.2f, 0);

    public void ShowIconFromItem(Item item, Transform targetTransform)
    {
        if (item == null || item.icon == null) return;

        GameObject prefab = Resources.Load<GameObject>("UI/FloatingIconUI");
        if (prefab != null && iconUIInstance == null)
        {
            iconUIInstance = Instantiate(prefab);
            iconImage = iconUIInstance.GetComponentInChildren<Image>();
            iconImage.sprite = item.icon;
            target = targetTransform;
        }
    }

    public void HideIcon()
    {
        if (iconUIInstance != null)
        {
            Destroy(iconUIInstance);
            iconUIInstance = null;
        }
    }

    void LateUpdate()
    {
        if (iconUIInstance != null && target != null)
        {
            iconUIInstance.transform.position = target.position + offset;
            iconUIInstance.transform.forward = Camera.main.transform.forward;
        }
    }
}
