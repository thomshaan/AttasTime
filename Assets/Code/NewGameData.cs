using UnityEngine;

public static class NewGameData
{
    public static Vector3 startPosition = new Vector3(0f, 1f, 0f);  // Your spawn position
    public static float startRotationY = 0f;                        // Your spawn rotation Y (degrees)
    public static int initialCoins = 100;
    public static int initialXP = 50;
    public static float initialTime = 6f;                           // 6 AM in your time system
    public static string initialScene = "WorldMain";

    public static void CreateFreshSave(int saveSlot, Vector3 position, float rotY = 0f)
    {
        SaveSystem.ClearInventory(saveSlot);

        SaveSystem.SavePlayerStats(
            coins: 100,
            xp: 50,
            sceneName: "WorldMain",
            position: position,
            rotationY: rotY,
            saveSlot: saveSlot
        );

        SaveSystem.SaveGameTime(6f, saveSlot);  // Simpan waktu awal (6 pagi)
        SaveSystem.SaveQuests("starterQuest", "NotStarted", saveSlot);
    }
}
