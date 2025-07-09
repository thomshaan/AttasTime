using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SleepBed : MonoBehaviour, IInteractable
{
    public Transform insideHouseSpawnPoint;
    public CanvasGroup fadeCanvasGroup;
    public float fadeDuration = 1f;
    public float wakeUpHour = 6f;

    private bool isSleeping = false;

    public string GetInteractionPrompt()
    {
        return "Tekan [E] untuk tidur";
    }

    public void Interact()
    {
        if (!isSleeping)
            StartCoroutine(SleepRoutine());
    }

    private IEnumerator SleepRoutine()
    {
        isSleeping = true;

        yield return StartCoroutine(Fade(0f, 1f));

        if (LightingManager.Instance != null)
            LightingManager.Instance.SetTimeOfDay(wakeUpHour);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && insideHouseSpawnPoint != null)
        {
            CharacterController controller = player.GetComponent<CharacterController>();
            if (controller != null)
                controller.enabled = false;

            player.transform.position = insideHouseSpawnPoint.position;
            player.transform.rotation = insideHouseSpawnPoint.rotation;

            if (controller != null)
                controller.enabled = true;
        }

        yield return new WaitForSeconds(0.5f);

        yield return StartCoroutine(Fade(1f, 0f));

        isSleeping = false;
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        if (fadeCanvasGroup == null)
            yield break;

        // Aktifkan CanvasGroup supaya terlihat
        fadeCanvasGroup.gameObject.SetActive(true);

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / fadeDuration);
            fadeCanvasGroup.alpha = alpha;
            yield return null;
        }

        fadeCanvasGroup.alpha = endAlpha;

        // Kalau fade selesai dan layar sudah terang (alpha = 0), matikan CanvasGroup supaya tidak blok input dll
        if (Mathf.Approximately(endAlpha, 0f))
        {
            fadeCanvasGroup.gameObject.SetActive(false);
        }
    }
}
