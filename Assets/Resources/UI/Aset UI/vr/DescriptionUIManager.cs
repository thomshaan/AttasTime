using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;

public class DescriptionUIManager : MonoBehaviour
{
    public XRRayInteractor rayInteractor;
    public Canvas popupCanvas;
    public TextMeshProUGUI objectNameText;

    void Update()
    {
        if (rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
        {
            Debug.Log("Ray kena: " + hit.collider.name);

            ObjectName info = hit.collider.GetComponent<ObjectName>();
            if (info != null)
            {
                Debug.Log("Objek punya ObjectInfo: " + info.objectName);
                objectNameText.text = info.objectName;
                popupCanvas.enabled = true;
                return;
            }
            else
            {
                Debug.Log("Objek tidak punya ObjectInfo!");
            }
        }

        popupCanvas.enabled = false;
    }


}
