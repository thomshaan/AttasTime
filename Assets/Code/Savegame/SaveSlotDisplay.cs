using UnityEngine;
using TMPro;

public class SaveSlotDisplay : MonoBehaviour
{
    public int slot; // Slot number: 1, 2, or 3
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI metaText;

    void Start()
    {
        UpdateSlotDisplay();
    }

    // Update tampilan info save slot berdasarkan data dari SaveSystem
    public void UpdateSlotDisplay()
    {
        PlayerSaveData data = SaveSystem.LoadPlayerStats(slot);

        if (data.coins == 0 && data.xp == 0 && data.lastPlayed == "Never")
        {
            titleText.text = "Empty Slot";
            metaText.text = "Start New Game";
        }
        else
        {
            titleText.text = $"Savegame {slot}";
            metaText.text = $"Coins: {data.coins} | XP: {data.xp}\nLast Played: {data.lastPlayed}";
        }
    }

    
}
