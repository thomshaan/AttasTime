using UnityEngine;

public class LogFilter : MonoBehaviour
{
    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        // Filter pesan warning shadow atlas size agar tidak diproses lebih lanjut
        if (type == LogType.Warning && logString.Contains("Reduced additional punctual light shadows resolution"))
        {
            // Abaikan log ini supaya tidak muncul di UI atau log custom
            return;
        }

        // Jika kamu punya log UI custom, tampilkan di sini.
        // Contoh: Debug.Log biasa tetap jalan
        
        Debug.unityLogger.Log(type, logString);
    }
}
