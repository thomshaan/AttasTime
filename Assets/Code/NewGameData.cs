using UnityEngine;

public static class NewGameData
{
    public static void CreateFreshSave()
    {
        int saveSlot = 1; // or assign dynamically

        // Position (default spawn)
        Vector3 startPosition = new Vector3(0f, 0f, 0f); // Update to your spawn point
        float startRotationY = 0f;

        // Inventory (empty)
        SaveSystem.ClearInventory(saveSlot);

        // Coins & XP
        SaveSystem.SavePlayerStats(
            coins: 10,
            xp: 5,
            sceneName: "WorldMain",
            position: startPosition,
            rotationY: startRotationY,
            saveSlot: saveSlot
        );

        // Optional: time, quests, etc.
        SaveSystem.SaveGameTime(8f, saveSlot); // 8:00 AM start

        Debug.Log("[NewGameData] New game save created.");
    }
}
