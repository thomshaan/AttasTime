using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DebugLogManager : MonoBehaviour
{
    [Header("Popup")]
    public RectTransform popupRect;
    public TextMeshProUGUI popupText;
    public float popupDuration = 3f;
    public float popupSlideTime = 0.3f;

    [Header("Full Log Panel")]
    public TextMeshProUGUI fullLogText;
    public int maxLogLines = 30;

    private Queue<string> logLines = new();
    private Coroutine popupRoutine;
    private Vector2 hiddenPos;
    private Vector2 shownPos;

    void Awake()
    {
        // Initialize popup position
        shownPos = popupRect.anchoredPosition;
        hiddenPos = shownPos + new Vector2(0, 150);
        popupRect.anchoredPosition = hiddenPos;
    }

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
        string formatted = logString;

        if (type == LogType.Warning)
            formatted = $"<color=yellow>{logString}</color>";
        else if (type == LogType.Error || type == LogType.Exception)
            formatted = $"<color=red>{logString}</color>";

        // Add to full log
        if (fullLogText != null)
        {
            logLines.Enqueue(formatted);
            if (logLines.Count > maxLogLines) logLines.Dequeue();
            fullLogText.text = string.Join("\n", logLines.ToArray());
        }

        // Show popup
        if (popupText != null)
        {
            popupText.text = logString;
            if (popupRoutine != null) StopCoroutine(popupRoutine);
            popupRoutine = StartCoroutine(AnimatePopup());
        }
    }

    IEnumerator AnimatePopup()
    {
        float t = 0f;
        while (t < popupSlideTime)
        {
            t += Time.deltaTime;
            popupRect.anchoredPosition = Vector2.Lerp(hiddenPos, shownPos, t / popupSlideTime);
            yield return null;
        }

        yield return new WaitForSeconds(popupDuration);

        t = 0f;
        while (t < popupSlideTime)
        {
            t += Time.deltaTime;
            popupRect.anchoredPosition = Vector2.Lerp(shownPos, hiddenPos, t / popupSlideTime);
            yield return null;
        }

        popupRect.anchoredPosition = hiddenPos;
    }
}
