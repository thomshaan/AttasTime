using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameUI : MonoBehaviour
{
    public SaveManager saveManager;
    public GameObject pauseMenu; // assign the pause panel from the scene

    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        pauseMenu.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
    }

    public void OnResume()
    {
        TogglePause();
    }

    public void OnSaveGame()
    {
        saveManager.SaveGame();
        Debug.Log("Game saved.");
    }

    public void OnReturnToMainMenu()
    {
        Time.timeScale = 1f; // reset time before switching
        SceneManager.LoadScene("MainMenu");
    }
}
