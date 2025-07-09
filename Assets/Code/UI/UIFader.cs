using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class UIFader : MonoBehaviour
{
    [Tooltip("CanvasGroup used for fade transitions")] 
    public CanvasGroup canvasGroup;
    [Tooltip("Duration of fade transitions in seconds")]
    public float fadeDuration = 1f;

    /// <summary>
    /// Event invoked when a fade completes.
    /// </summary>
    public event Action OnFadeComplete;

    private void Reset()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    /// <summary>
    /// Fades the CanvasGroup to targetAlpha over fadeDuration seconds.
    /// </summary>
    public IEnumerator Fade(float targetAlpha)
    {
        float startAlpha = canvasGroup.alpha;
        float elapsed = 0f;

        // Block interactions during fade
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = true;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
        canvasGroup.interactable = targetAlpha > 0f;
        canvasGroup.blocksRaycasts = targetAlpha > 0f;

        OnFadeComplete?.Invoke();
    }

    public IEnumerator FadeIn()
    {
        yield return StartCoroutine(Fade(1f));
    }

    public IEnumerator FadeOut()
    {
        yield return StartCoroutine(Fade(0f));
    }
}
