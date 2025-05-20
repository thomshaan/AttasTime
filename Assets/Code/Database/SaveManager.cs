using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class SaveManager : MonoBehaviour
{
    public SaveSystem saveSystem;
    public Inventory inventory;
    public PlayerStats playerStats;
    public GameObject player;
    public QuestManager questManager;

    public int currentSaveSlot = 1;

    public void SaveGame()
    {
        // Save Inventory
        List<SimpleItemSlot> simpleInventory = inventory.ToSimpleItemList();
        saveSystem.SaveInventory(simpleInventory, currentSaveSlot);

        // Save Player Stats
        saveSystem.SavePlayerStats(
            playerStats.coins,
            playerStats.xp,
            SceneManager.GetActiveScene().name,
            player.transform.position,
            player.transform.eulerAngles.y,
            currentSaveSlot
        );

        // Save Quest
        (string questId, string questState) = questManager.GetQuestStateData();
        saveSystem.SaveQuests(questId, questState, currentSaveSlot);

        // Save Time (LightingManager TimeOfDay)
        float time = LightingManager.Instance.TimeOfDay;
        saveSystem.SaveGameTime(time, currentSaveSlot);

        Debug.Log($"[SaveManager] Game saved to slot {currentSaveSlot}");
        Debug.Log("[Save] Position: " + player.transform.position);
    }

    public void LoadGame()
    {
        // Load Inventory
        List<SimpleItemSlot> simpleInventory = saveSystem.LoadInventory(currentSaveSlot);
        inventory.LoadFromSimpleItemList(simpleInventory);

        // Load Player Stats
        var stats = saveSystem.LoadPlayerStats(currentSaveSlot);
        playerStats.coins = stats.coins;
        playerStats.xp = stats.xp;

        Debug.Log("[Load] Loaded Position: " + stats.position);
        Debug.Log("[Load] Player object: " + player?.name);

        // Move Player (with CharacterController support)
        CharacterController controller = player.GetComponent<CharacterController>();
        if (controller != null)
        {
            controller.enabled = false; // disable to prevent conflict
            player.transform.position = stats.position;
            player.transform.rotation = Quaternion.Euler(0, stats.rotY, 0);
            controller.enabled = true;
            Debug.Log("[Load] Moved player using CharacterController-safe method.");
        }
        else
        {
            player.transform.position = stats.position;
            player.transform.rotation = Quaternion.Euler(0, stats.rotY, 0);
            Debug.Log("[Load] Moved player directly (no CharacterController).");
        }

        // Load Quest
        var questData = saveSystem.LoadQuests(currentSaveSlot);
        questManager.LoadQuestStateData(questData.questId, questData.questState);

        // Load Time
        float timeOfDay = saveSystem.LoadGameTime(currentSaveSlot);
        LightingManager.Instance.SetTimeOfDay(timeOfDay);

        Debug.Log($"[SaveManager] Game loaded from slot {currentSaveSlot}");
    }
}
