using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void ButtonNewGame()
    {
        SceneManager.LoadScene("WorldMain");
    }

    public void ButtonCredits()
    {
        SceneManager.LoadScene("Credits");
    }

    public void ButtonQuit()
    {
        Application.Quit();
    }
}
