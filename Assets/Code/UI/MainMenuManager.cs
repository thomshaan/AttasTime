using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public SaveSystem saveSystem; // Drag the SaveSystem GameObject here in the Inspector

    public void ButtonNewGame()
    {
        // Optionally clear previous PlayerPrefs
        PlayerPrefs.SetInt("LastSaveSlot", 1);
        SceneManager.LoadScene("WorldMain");
    }

    public void ButtonOptions()
    {
        SceneManager.LoadScene("Options");
    }

    public void ButtonQuit()
    {
        Application.Quit();
    }

    public void ButtonResumeGame()
    {
        int lastSlot = PlayerPrefs.GetInt("LastSaveSlot", 1);
        var stats = saveSystem.LoadPlayerStats(lastSlot);

        if (!string.IsNullOrEmpty(stats.scene))
        {
            SceneManager.LoadScene(stats.scene);
        }
        else
        {
            Debug.LogWarning("[MainMenu] No saved scene found.");
        }
    }

    public void ButtonLoadGame(int slot)
    {
        PlayerPrefs.SetInt("LastSaveSlot", slot);
        var stats = saveSystem.LoadPlayerStats(slot);

        if (!string.IsNullOrEmpty(stats.scene))
        {
            SceneManager.LoadScene(stats.scene);
        }
        else
        {
            Debug.LogWarning($"[MainMenu] No saved scene found in slot {slot}");
        }
    }
}
