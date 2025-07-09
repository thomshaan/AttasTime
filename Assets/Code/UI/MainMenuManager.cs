using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public string worldSceneName = "WorldMain"; // Change to your actual scene name

    public void Start()
    {
        AudioManager.Instance.StopMusic();
        AudioManager.Instance.PlayMenuMusic();
        
    }

    public void ButtonNewGame()
    {
        AudioManager.Instance.PlaySFX("klik");
        SceneManager.LoadScene("SelectSave");
    }

    public void ButtonLoadGame(int slot = 1)
    {
        AudioManager.Instance.PlaySFX("klik");
        SceneManager.LoadScene("LoadGame");
    }

    public void ButtonQuit()
    {
        AudioManager.Instance.PlaySFX("klik");
        Application.Quit();
    }

    public void ButtonSettingsGame()
    {
        AudioManager.Instance.PlaySFX("klik");
        SceneManager.LoadScene("Settings");
    }

    public void ButtonCreditsGame()
    {
        AudioManager.Instance.PlaySFX("klik");
        SceneManager.LoadScene("Credits");
    }

}
