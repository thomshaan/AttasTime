using UnityEngine;

public class SaveSlotManager : MonoBehaviour
{
    public SaveSlotDisplay[] slotDisplays;

    void Start()
    {
        RefreshAllSlots();
    }

    // Panggil untuk update tampilan semua slot save
    public void RefreshAllSlots()
    {
        foreach (var slotDisplay in slotDisplays)
        {
            slotDisplay.UpdateSlotDisplay();
        }
    }

    // Fungsi dipanggil dari UI tombol delete, hapus slot dan refresh UI
    public void DeleteSlot(int slot)
    {
        AudioManager.Instance.PlaySFX("klik");
        SaveSystem.DeleteSaveSlot(slot);  // langsung panggil SaveSystem statis tanpa SaveManager

        // Jika ingin reset slot ke fresh save default, panggil juga NewGameData:
        NewGameData.InitializeStartPosition("SpawnRumah");
        NewGameData.CreateFreshSave(slot, NewGameData.startPosition, NewGameData.startRotationY);

        RefreshAllSlots();
    }

}
