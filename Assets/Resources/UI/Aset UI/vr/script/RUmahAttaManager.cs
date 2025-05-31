using UnityEngine;
using UnityEngine.SceneManagement;

public class RUmahAttaManager : MonoBehaviour
{
    public GameObject gameplayCanvas;
    public GameObject popupName;
    public GameObject popupPause;
    public Transform playerXR; // XR Origin

    private bool isPaused = false;

    public enum RumahType { Atta, Gadang }
    public RumahType rumahType;


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

        if (rumahType == RumahType.Atta)
        {
            PlayerPrefs.SetInt("BackFromHouse", 1);
        }
        else if (rumahType == RumahType.Gadang)
        {
            PlayerPrefs.SetInt("BackFromGadang", 1);
        }

        SceneManager.LoadScene(sceneName);
    }

}
