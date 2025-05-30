using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuStateController : MonoBehaviour
{
    public GameObject menuCanvas;
    public GameObject gameplayCanvas;
    public GameObject popupName;
    public GameObject popupPause;
    public GameObject rumahAttaCanvas;
    public Transform playerXR;       // XR Origin
    public Transform cameraOffset;   // Camera Offset in XR Rig

    private bool isPaused = false;

    void Start()
    {
        // Cek apakah kembali dari rumah
        bool backFromHouse = PlayerPrefs.GetInt("BackFromHouse", 0) == 1;

        if (backFromHouse)
        {
            menuCanvas.SetActive(false);
            gameplayCanvas.SetActive(true);
            popupName.SetActive(true);
            popupPause.SetActive(false);
            rumahAttaCanvas.SetActive(true);

            // Reset flag agar tidak terus-menerus
            PlayerPrefs.DeleteKey("BackFromHouse");
        }
        else
        {
            menuCanvas.SetActive(true);
            gameplayCanvas.SetActive(false);
            popupName.SetActive(false);
            popupPause.SetActive(false);
            rumahAttaCanvas.SetActive(false);
        }

        // Set posisi karakter kalau ada yang disimpan
        if (PlayerPrefs.HasKey("PrevPosX"))
        {
            float x = PlayerPrefs.GetFloat("PrevPosX");
            float y = PlayerPrefs.GetFloat("PrevPosY");
            float z = PlayerPrefs.GetFloat("PrevPosZ");

            playerXR.position = new Vector3(x, y, z);

            PlayerPrefs.DeleteKey("PrevPosX");
            PlayerPrefs.DeleteKey("PrevPosY");
            PlayerPrefs.DeleteKey("PrevPosZ");
        }
    }


    public void StartGame()
    {
        menuCanvas.SetActive(false);
        gameplayCanvas.SetActive(true);
        popupName.SetActive(true);
        popupPause.SetActive(false);
        rumahAttaCanvas.SetActive(true);
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

    public void QuitGame()
    {
        Application.Quit();
    }

    public void LoadScene(string sceneName)
    {
        Time.timeScale = 1;

        // Simpan posisi karakter sebelum masuk ke rumah
        if (playerXR != null)
        {
            Vector3 pos = playerXR.position;
            PlayerPrefs.SetFloat("PrevPosX", pos.x);
            PlayerPrefs.SetFloat("PrevPosY", pos.y);
            PlayerPrefs.SetFloat("PrevPosZ", pos.z);
        }

        SceneManager.LoadScene(sceneName);
    }
}
