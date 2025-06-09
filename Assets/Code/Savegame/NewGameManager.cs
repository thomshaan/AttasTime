using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using UnityEngine.UI;

public class NewGameManager : MonoBehaviour
{
    [Header("Cutscene Settings")]
    public VideoPlayer cutsceneVideoPlayer;  // VideoPlayer component to play the cutscene
    public RawImage cutsceneRawImage;        // RawImage to display the video on the UI
    public string worldSceneName = "WorldMain"; // The world scene to load after cutscene ends

    // Fungsi ini dipanggil saat user pilih save slot di UI New Game
    public void StartNewGame(int saveSlot)
    {
        AudioManager.Instance.StopMusic(); // Stop any current background music

        Debug.Log($"Start New Game overwrite slot {saveSlot}");

        // 1. Show the cutscene panel (UI)
        cutsceneRawImage.gameObject.SetActive(true);  // Enable the RawImage to show the video
        cutsceneVideoPlayer.gameObject.SetActive(true);  // Enable the VideoPlayer component

        // 2. Play the cutscene
        PlayCutscene();
        NewGameData.CreateFreshSave(saveSlot, NewGameData.startPosition, NewGameData.startRotationY);

        // 2. Set the current save slot
        SaveManager.currentSaveSlot = saveSlot;

        // 3. After the cutscene ends, the following steps will be handled by the VideoPlayer's event
        cutsceneVideoPlayer.loopPointReached += (vp) => OnCutsceneEnd(saveSlot);
    }

    private void PlayCutscene()
    {
        cutsceneVideoPlayer.Play();  // Play the full MP4 video
    }

    private void OnCutsceneEnd(int saveSlot)
    {
        // 1. Overwrite save data with fresh data
        

        // 3. Load the world scene after the cutscene ends
        SceneManager.LoadScene(worldSceneName);

        // Optionally hide the cutscene panel after it's finished (if needed)
        cutsceneRawImage.gameObject.SetActive(false);
        cutsceneVideoPlayer.gameObject.SetActive(false);
    }

    public void BackToMenu()
    {
        AudioManager.Instance.PlaySFX("klik");

        // Load the Main Menu scene
        SceneManager.LoadScene("MainMenu");
    }
}
