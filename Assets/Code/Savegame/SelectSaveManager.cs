using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectSaveManager : MonoBehaviour
{
    public SaveConfirmationPopup popup;

    private Vector3 initialSpawnPosition = new Vector3(187f, 83f, 112f); // initial spawn for new game
    private float initialRotationY = 180f;

    public void OnSlotClicked(int slot)
    {
        var dataExists = SaveSystem.TryLoadPlayerStats(slot, out var stats);

        if (!dataExists)
        {
            
            // New game: create fresh save with initial position
            SaveManager.currentSaveSlot = slot;
            NewGameData.CreateFreshSave(slot, initialSpawnPosition, initialRotationY);
            SceneManager.LoadScene(NewGameData.initialScene);
        }
        else
        {
            // Existing save: show confirmation popup
            popup.Show(slot);
        }
    }

    // Called by confirmation popup "Yes" to confirm loading existing save
    public void LoadSlot(int slot)
    {
        SaveManager.currentSaveSlot = slot;
        // DO NOT create fresh save here! Just load existing save data
        SceneManager.LoadScene("WorldMain");
    }

    public void OnBackButtonClicked()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
