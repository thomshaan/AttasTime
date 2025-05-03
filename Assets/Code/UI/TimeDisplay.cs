using TMPro;
using UnityEngine;

public class TimeDisplayUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI timePeriodText; // New TMP text for showing time of day period

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

            // Determine the time of day period
            string period = GetTimePeriod(time);
            timePeriodText.text = period;
        }
    }

    private string GetTimePeriod(float time)
    {
        if (time >= 5f && time < 7f)
            return "Dawn";
        else if (time >= 7f && time < 18f)
            return "Daylight";
        else if (time >= 18f && time < 19f)
            return "Sunset";
        else
            return "Nighttime";
    }
}
