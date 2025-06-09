using UnityEngine;
using UnityEngine.SceneManagement;

public class NewGameManager : MonoBehaviour
{
    // Fungsi ini dipanggil saat user pilih save slot di UI New Game
    public void StartNewGame(int saveSlot)
    {
        AudioManager.Instance.StopMusic();
        Debug.Log($"Start New Game overwrite slot {saveSlot}");

        // 1. Overwrite save data dengan data awal
        NewGameData.CreateFreshSave(saveSlot, NewGameData.startPosition, NewGameData.startRotationY);

        // 2. Set current save slot ke yang dipilih
        SaveManager.currentSaveSlot = saveSlot;

        // 3. Load scene utama dengan fresh data
        SceneManager.LoadScene(NewGameData.initialScene);
    }

    public void BackToMenu()
    {AudioManager.Instance.PlaySFX("klik");

        // 3. Load scene utama dengan fresh data
        SceneManager.LoadScene("MainMenu");
    }
}
