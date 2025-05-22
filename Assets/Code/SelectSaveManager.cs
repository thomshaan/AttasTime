using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectSaveManager : MonoBehaviour
{
    public SaveConfirmationPopup popup;

    public void OnSlotClicked(int slot)
    {
        var data = SaveSystem.LoadPlayerStats(slot);
        if (data.lastPlayed == "Never")
        {
            SaveManager.currentSaveSlot = slot;
            NewGameData.CreateFreshSave();
            SceneManager.LoadScene("WorldMain");
        }
        else
        {
            popup.Show(slot);
        }
    }

    // Add this method to be called by the confirmation popup when the user confirms overwriting
    public void LoadSlot(int slot)
    {
        SaveManager.currentSaveSlot = slot;
        NewGameData.CreateFreshSave();
        SceneManager.LoadScene("WorldMain");
    }
}
