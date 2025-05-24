using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class ResumeGameManager : MonoBehaviour
{
    public SaveConfirmationPopup popup;      // Confirmation dialog UI
    public Transform playerSpawnPoint;       // Default spawn point if needed
    public SaveSystem saveSystem;            // Reference to your SaveSystem instance

    // Called when player clicks a save slot button
    public void OnSlotClicked(int slot)
    {
        // Try to load player stats metadata for this slot
        var data = SaveSystem.LoadPlayerStats(slot);

        if (data.coins == 0 && data.xp == 0 && data.position == Vector3.zero)
        {
            // Slot empty or no valid save found
            Debug.LogWarning($"Save slot {slot} is empty.");
            // Optionally show message or disable button
            return;
        }

        // Slot has a valid save → show confirmation popup to resume
        popup.Show(slot);
    }

    // Called when player confirms "Yes" on confirmation popup
    public void LoadSlot(int slot)
    {
        SaveManager.currentSaveSlot = slot;
        SceneManager.LoadScene("WorldMain");  // Load your main game scene
    }

    // Called when Back button clicked
    public void OnBackButtonClicked()
    {
        SceneManager.LoadScene("MainMenu");  // Back to main menu scene
    }
}
