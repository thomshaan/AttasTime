using UnityEngine;
using UnityEngine.UI;

public class SellerIconUI : MonoBehaviour
{
    private GameObject iconUIInstance;
    private Image iconImage;
    private Transform target;
    public Vector3 offset = new Vector3(0, 2.2f, 0);

    public void Initialize(Transform targetTransform, Sprite itemIcon)
    {
        target = targetTransform;

        if (iconUIInstance == null)
        {
            GameObject prefab = Resources.Load<GameObject>("UI/SellerIconUI");
            if (prefab != null)
            {
                iconUIInstance = Instantiate(prefab);
                iconImage = iconUIInstance.GetComponentInChildren<Image>();
            }
        }

        if (iconImage != null)
        {
            iconImage.sprite = itemIcon;
        }

        iconUIInstance.SetActive(true);
    }

    public void ShowIcon(Sprite iconSprite, Transform targetTransform)
    {
        if (iconSprite == null) return;

        if (iconUIInstance == null)
        {
            GameObject prefab = Resources.Load<GameObject>("UI/SellerIconUI");
            if (prefab != null)
            {
                iconUIInstance = Instantiate(prefab);
                iconImage = iconUIInstance.GetComponentInChildren<Image>();
            }
        }

        if (iconImage != null)
        {
            iconImage.sprite = iconSprite;
        }

        target = targetTransform;
        iconUIInstance.SetActive(true);
    }

    public void HideIcon()
    {
        if (iconUIInstance != null)
        {
            iconUIInstance.SetActive(false);
            target = null; // Lepaskan referensi target agar icon tidak update lagi
        }
    }

    void LateUpdate()
    {
        if (iconUIInstance != null)
        {
            if (target == null)
            {
                iconUIInstance.SetActive(false);
                return;
            }

            iconUIInstance.transform.position = target.position + offset;
            iconUIInstance.transform.forward = Camera.main.transform.forward;
        }
    }
}
