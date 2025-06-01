using UnityEngine;
using System.Collections;

public class UIFader : MonoBehaviour
{
    public RectTransform rectTransform;

    public float slideDuration = 0.5f;
    public float slideDistance = 200f;

    private Vector2 originalPosition;
    private Coroutine currentCoroutine;

    private void Awake()
    {
        if (rectTransform == null) rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition;
    }

    // Slide keluar (ke kiri atau kanan)
    public void SlideOut(bool slideLeft)
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }
        Vector2 targetPos = originalPosition + (slideLeft ? Vector2.left : Vector2.right) * slideDistance;
        currentCoroutine = StartCoroutine(SlideRoutine(targetPos));
    }

    // Slide kembali ke posisi awal
    public void SlideIn()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }
        currentCoroutine = StartCoroutine(SlideRoutine(originalPosition));
    }

    private IEnumerator SlideRoutine(Vector2 targetPos)
    {
        Vector2 startPos = rectTransform.anchoredPosition;
        float elapsed = 0f;

        while (elapsed < slideDuration)
        {
            elapsed += Time.deltaTime;
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, targetPos, elapsed / slideDuration);
            yield return null;
        }
        rectTransform.anchoredPosition = targetPos;
    }
}
