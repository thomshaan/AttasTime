using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResumeGameManager : MonoBehaviour
{
    public SaveConfirmationPopup popup;          // Confirmation dialog UI
    public Transform playerSpawnPoint;           // Default spawn point if needed
    public SaveSystem saveSystem;                // Reference to SaveSystem instance

    public SaveSlotButton[] saveSlotButtons;     // Assign semua tombol slot di inspector

    void Start()
    {
        RefreshAllSlotButtons();
    }

    public void RefreshAllSlotButtons()
    {
        foreach (var slotBtn in saveSlotButtons)
        {
            slotBtn.UpdateButtonInteractable();
        }
    }

    // Called when player clicks a save slot button
    public void OnSlotClicked(int slot)
    {
        var data = SaveSystem.LoadPlayerStats(slot);

        if (data.coins == 0 && data.xp == 0 && data.position == Vector3.zero)
        {
            Debug.LogWarning($"Slot {slot} kosong.");
            return;
        }

        popup.Show(slot);
    }

    // Called when player confirms "Yes" on confirmation popup
    public void LoadSlot(int slot)
    {
        SaveManager.currentSaveSlot = slot;
        SceneManager.LoadScene("WorldMain");  // Load main game scene
    }

    // Called when Back button clicked
    public void OnBackButtonClicked()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
