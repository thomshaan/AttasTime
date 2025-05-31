using UnityEngine;
using UnityEngine.UI;

public class SellerIconUI : MonoBehaviour
{
    public Image iconImage;
    public Vector3 offset = new Vector3(0, 2.2f, 0); // posisi di atas kepala NPC
    private Transform target;

    public void Initialize(Transform targetTransform, Sprite itemIcon)
    {
        target = targetTransform;
        iconImage.sprite = itemIcon;
    }

    void LateUpdate()
    {
        if (target != null)
        {
            transform.position = target.position + offset;
            transform.forward = Camera.main.transform.forward; // agar selalu menghadap kamera
        }
    }
}
