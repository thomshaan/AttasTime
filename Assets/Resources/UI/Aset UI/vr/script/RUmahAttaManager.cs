using UnityEngine;
using UnityEngine.SceneManagement;

public class RUmahAttaManager : MonoBehaviour
{
    public GameObject gameplayCanvas;
    public GameObject popupName;
    public GameObject popupPause;
    public Transform playerXR; // XR Origin

    private bool isPaused = false;

    void Start()
    {
        gameplayCanvas.SetActive(true);
        popupName.SetActive(true);
        popupPause.SetActive(false);
    }

    public void PauseGame()
    {
        popupPause.SetActive(true);
        Time.timeScale = 0;
        isPaused = true;
    }

    public void ResumeGame()
    {
        popupPause.SetActive(false);
        Time.timeScale = 1;
        isPaused = false;
    }

    public void LoadScene(string sceneName)
    {
        Time.timeScale = 1;

        // Simpan posisi XR saat keluar dari rumah jika perlu
        if (playerXR != null)
        {
            Vector3 pos = playerXR.position;
            PlayerPrefs.SetFloat("PrevPosX", pos.x);
            PlayerPrefs.SetFloat("PrevPosY", pos.y);
            PlayerPrefs.SetFloat("PrevPosZ", pos.z);
        }

        PlayerPrefs.SetInt("BackFromHouse", 1);

        SceneManager.LoadScene(sceneName);
    }
}
