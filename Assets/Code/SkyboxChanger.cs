using UnityEngine;

public class SkyboxSwitcher : MonoBehaviour
{
    [Header("Skybox Materials")]
    [SerializeField] private Material daySkybox;
    [SerializeField] private Material nightSkybox;

    [Header("Time Settings")]
    [SerializeField, Range(0, 24)] private float dayStart = 6f;    // 6 AM
    [SerializeField, Range(0, 24)] private float nightStart = 18f; // 6 PM

    private bool isNight = false;

    private void Update()
    {
        if (LightingManager.Instance == null) return;

        float currentTime = LightingManager.Instance.TimeOfDay;

        // Switch to night
        if (!isNight && (currentTime >= nightStart || currentTime < dayStart))
        {
            RenderSettings.skybox = nightSkybox;
            DynamicGI.UpdateEnvironment(); // Optional: updates lighting from skybox
            isNight = true;
        }

        // Switch to day
        else if (isNight && currentTime >= dayStart && currentTime < nightStart)
        {
            RenderSettings.skybox = daySkybox;
            DynamicGI.UpdateEnvironment(); // Optional
            isNight = false;
        }
    }
}
