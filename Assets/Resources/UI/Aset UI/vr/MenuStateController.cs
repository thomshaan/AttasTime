using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuStateController : MonoBehaviour
{
    public GameObject menuCanvas;     // Assign ke MainMenuCanvas
    public GameObject gameplayCanvas;
    public GameObject popupName;
    public GameObject popupPause;
    bool isPaused = false;

    public Transform playerXR; // Assign XR Origin atau karakter utama

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private bool hasStoredInitialPosition = false;

    public Transform cameraOffset; // Biasanya bernama "Camera Offset" dalam XR Rig
    private Vector3 initialCamOffsetPos;
    private Quaternion initialCamOffsetRot;


    void Start()
    {
        // Pastikan menu muncul saat game dimulai
        menuCanvas.SetActive(true);
        gameplayCanvas.SetActive(false);
        popupName.SetActive(false);
        popupPause.SetActive(false);

        // Simpan posisi awal player XR saat pertama kali aplikasi dijalankan
        if (playerXR != null && !hasStoredInitialPosition)
        {
            initialPosition = playerXR.position;
            initialRotation = playerXR.rotation;
            hasStoredInitialPosition = true;
        }

        if (cameraOffset != null)
        {
            initialCamOffsetPos = cameraOffset.localPosition;
            initialCamOffsetRot = cameraOffset.localRotation;
        }

    }

    public void StartGame()
    {
        menuCanvas.SetActive(false);
        gameplayCanvas.SetActive(true);
        popupName.SetActive(true);
        popupPause.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Keluar game...");
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

    public void BackToMainMenu()
    {
        menuCanvas.SetActive(true);
        gameplayCanvas.SetActive(false);
        popupName.SetActive(false);
        popupPause.SetActive(false);

        Time.timeScale = 1; // Pastikan tidak dalam kondisi pause

        // Reset posisi player ke awal aplikasi
        if (playerXR != null)
        {
            playerXR.position = initialPosition;
            playerXR.rotation = initialRotation;
        }

        if (cameraOffset != null)
        {
            cameraOffset.localPosition = initialCamOffsetPos;
            cameraOffset.localRotation = initialCamOffsetRot;
        }

    }

    public void LoadScene(string sceneName)
    {
        Time.timeScale = 1; // Pastikan tidak dalam keadaan pause
        SceneManager.LoadScene(sceneName);
    }

}
