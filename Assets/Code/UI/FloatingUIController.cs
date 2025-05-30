using UnityEngine;
using UnityEngine.UI;

public class FloatingIconController : MonoBehaviour
{
    private GameObject iconUIInstance;
    private Image iconImage;
    private Transform target;
    public Vector3 offset = new Vector3(0, 2.2f, 0);

    public void ShowIcon(Sprite iconSprite, Transform targetTransform)
    {
        if (iconSprite == null) return;

        if (iconUIInstance == null)
        {
            GameObject prefab = Resources.Load<GameObject>("UI/FloatingIconUI");
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
            // atau Destroy(iconUIInstance); tergantung kebutuhan
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
