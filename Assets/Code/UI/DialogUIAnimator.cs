using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DialogUIAnimator : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public RectTransform dialogPanel;

    public float animationDuration = 0.5f;
    public Vector2 hiddenPosition = new Vector2(0, -500);
    public Vector2 shownPosition = Vector2.zero;

    private Coroutine currentCoroutine;

    private void Awake()
    {
        dialogPanel.anchoredPosition = hiddenPosition;
        canvasGroup.alpha = 0f;
        dialogPanel.gameObject.SetActive(false);
    }

    public void Show()
    {
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        dialogPanel.gameObject.SetActive(true);
        currentCoroutine = StartCoroutine(Animate(true));
    }

    public void Hide()
    {
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        currentCoroutine = StartCoroutine(Animate(false));
    }

    private IEnumerator Animate(bool showing)
    {
        float elapsed = 0f;
        Vector2 startPos = showing ? hiddenPosition : shownPosition;
        Vector2 targetPos = showing ? shownPosition : hiddenPosition;

        float startAlpha = showing ? 0f : 1f;
        float targetAlpha = showing ? 1f : 0f;

        canvasGroup.alpha = startAlpha;
        dialogPanel.anchoredPosition = startPos;

        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / animationDuration);

            dialogPanel.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);

            yield return null;
        }

        dialogPanel.anchoredPosition = targetPos;
        canvasGroup.alpha = targetAlpha;

        if (!showing)
            dialogPanel.gameObject.SetActive(false);
    }
}
