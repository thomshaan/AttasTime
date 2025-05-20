using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SplashLoader : MonoBehaviour
{
    public GameObject LoadingScreen;
    public Image LoadingBarfill;

    void Start()
    {
        // Mulai loading otomatis saat scene aktif
        StartCoroutine(LoadSceneAsync(1));
    }

    IEnumerator LoadSceneAsync(int sceneId)
    {
        // Tampilkan UI loading
        LoadingScreen.SetActive(true);
        yield return new WaitForSeconds(0.1f); // biar UI sempat muncul

        // Mulai load scene target
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneId);

        // Selama loading belum selesai
        while (!operation.isDone)
        {
            float progressValue = Mathf.Clamp01(operation.progress / 0.9f); // Normalisasi progress
            LoadingBarfill.fillAmount = progressValue;
            yield return null;
        }
    }
}
