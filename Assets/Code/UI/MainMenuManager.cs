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
        SaveManager.currentSaveSlot = slot;
        SceneManager.LoadScene(worldSceneName);
    }

    public void ButtonQuit()
    {
        Application.Quit();
    }
}
