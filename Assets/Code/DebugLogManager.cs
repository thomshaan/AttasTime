using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DebugLogManager : MonoBehaviour
{
    [Header("Activity Log UI (Text A)")]
    public RectTransform activityPopupRect;
    public TextMeshProUGUI activityPopupText;
    public float activityPopupDuration = 2.5f;
    public float activityPopupSlideTime = 0.3f;

    [Header("Debug Log Panel (Text B)")]
    public TextMeshProUGUI debugLogText;
    public float debugLogLifetime = 6f; // Durasi tiap log tampil
    public int maxDebugLines = 30;

    public static DebugLogManager Instance { get; private set; }

    private List<LogEntry> debugEntries = new();
    private Coroutine activityRoutine;
    private Vector2 hiddenPos;
    private Vector2 shownPos;

    private class LogEntry
    {
        public string message;
        public float timestamp;

        public LogEntry(string msg, float time)
        {
            message = msg;
            timestamp = time;
        }
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        shownPos = activityPopupRect.anchoredPosition;
        hiddenPos = shownPos + new Vector2(0, 150);
        activityPopupRect.anchoredPosition = hiddenPos;
    }

    void OnEnable()
    {
        Application.logMessageReceived += HandleDebugLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleDebugLog;
    }

    public void ShowLog(string message)
    {
        if (activityPopupText != null)
        {
            activityPopupText.text = message;
            if (activityRoutine != null) StopCoroutine(activityRoutine);
            activityRoutine = StartCoroutine(AnimateActivityPopup());
        }
    }

    void HandleDebugLog(string logString, string stackTrace, LogType type)
    {
        if (debugLogText == null) return;

        string trimmed = logString.Length > 200 ? logString.Substring(0, 200) + "..." : logString;
        string formatted = trimmed;

        if (type == LogType.Warning)
            formatted = $"<color=yellow>{trimmed}</color>";
        else if (type == LogType.Error || type == LogType.Exception)
            formatted = $"<color=red>{trimmed}</color>";

        debugEntries.Add(new LogEntry(formatted, Time.time));

        if (debugEntries.Count > maxDebugLines)
            debugEntries.RemoveAt(0);

        UpdateDebugLogUI();
    }

    void Update()
    {
        // Hapus log lama berdasarkan waktu
        bool updated = false;
        float now = Time.time;
        for (int i = debugEntries.Count - 1; i >= 0; i--)
        {
            if (now - debugEntries[i].timestamp > debugLogLifetime)
            {
                debugEntries.RemoveAt(i);
                updated = true;
            }
        }

        if (updated)
            UpdateDebugLogUI();
    }

    void UpdateDebugLogUI()
    {
        List<string> msgs = new();
        foreach (var entry in debugEntries)
        {
            msgs.Add(entry.message);
        }
        debugLogText.text = string.Join("\n", msgs);
    }

    IEnumerator AnimateActivityPopup()
    {
        float t = 0f;
        while (t < activityPopupSlideTime)
        {
            t += Time.deltaTime;
            activityPopupRect.anchoredPosition = Vector2.Lerp(hiddenPos, shownPos, t / activityPopupSlideTime);
            yield return null;
        }

        yield return new WaitForSeconds(activityPopupDuration);

        t = 0f;
        while (t < activityPopupSlideTime)
        {
            t += Time.deltaTime;
            activityPopupRect.anchoredPosition = Vector2.Lerp(shownPos, hiddenPos, t / activityPopupSlideTime);
            yield return null;
        }

        activityPopupRect.anchoredPosition = hiddenPos;
    }
}
