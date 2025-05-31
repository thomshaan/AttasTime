using UnityEngine;
using UnityEngine.UI;

public class SaveSlotUI : MonoBehaviour
{
    public int slotNumber;
    public Button deleteButton;

    private SaveSlotManager slotManager;

    void Start()
    {
        slotManager = FindObjectOfType<SaveSlotManager>();

        if (deleteButton != null)
        {
            deleteButton.onClick.AddListener(OnDeleteButtonClicked);
        }
    }

    public void OnDeleteButtonClicked()
    {
        // Optional: tampilkan konfirmasi di sini

        if (slotManager != null)
        {
            slotManager.DeleteSlot(slotNumber);
            Debug.Log($"[SaveSlotUI] Deleted save slot {slotNumber}");
        }
    }
}
