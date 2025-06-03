using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public string worldSceneName = "WorldMain"; // Change to your actual scene name

    public void ButtonNewGame()
    {
        SceneManager.LoadScene("SelectSave");
    }

    public void ButtonLoadGame(int slot = 1)
    {
        SceneManager.LoadScene("LoadGame");
    }

    public void ButtonQuit()
    {
        Application.Quit();
    }

    public void ButtonSettingsGame()
    {
        SceneManager.LoadScene("Settings");
    }

    public void ButtonCreditsGame()
    {
        SceneManager.LoadScene("Credits");
    }

}
