using UnityEngine;
using UnityEngine.UI;

public class SaveSlotButton : MonoBehaviour
{
    public int slotNumber;
    public Button slotButton;

    void Start()
    {
        UpdateButtonInteractable();
    }

    public void UpdateButtonInteractable()
    {
        var data = SaveSystem.LoadPlayerStats(slotNumber);
        bool hasSave = !(data.coins == 0 && data.xp == 0 && data.position == Vector3.zero);
        slotButton.interactable = hasSave;
    }
}
