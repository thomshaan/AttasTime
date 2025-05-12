using UnityEngine;

public class FlashlightToggle : MonoBehaviour
{
    [SerializeField] private GameObject flashlight;

    public void ToggleFlashlight()
    {
        if (flashlight != null)
        {
            flashlight.SetActive(!flashlight.activeSelf);
        }
    }
}
