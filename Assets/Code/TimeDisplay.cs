using TMPro;
using UnityEngine;

public class TimeDisplayUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timeText;

    private void Update()
    {
        if (LightingManager.Instance != null)
        {
            float time = LightingManager.Instance.TimeOfDay;

            // Convert float time (e.g. 13.5) to hour and minute
            int hours = Mathf.FloorToInt(time);
            int minutes = Mathf.FloorToInt((time - hours) * 60);

            // Format time as "HH:MM"
            string formattedTime = $"{hours:00}:{minutes:00}";
            timeText.text = formattedTime;
        }
    }
}
