using UnityEngine;

public static class NewGameData
{
    public static Vector3 startPosition = Vector3.zero;
    public static float startRotationY = 0f;
    public static string initialScene = "WorldMain";  // scene awal saat new game

    // Method untuk inisialisasi posisi dan rotasi player saat mulai new game
    public static void InitializeStartPosition(string spawnID)
    {
        SpawnPoint[] spawns = GameObject.FindObjectsOfType<SpawnPoint>();
        foreach (var sp in spawns)
        {
            if (sp.spawnID == spawnID)
            {
                startPosition = sp.transform.position;
                startRotationY = sp.transform.eulerAngles.y;
                Debug.Log($"[NewGameData] Start position set to spawn '{spawnID}' at {startPosition}");
                return;
            }
        }

        // Jika spawnID tidak ditemukan, pakai default (0,0,0)
        Debug.LogWarning($"[NewGameData] Spawn point '{spawnID}' not found. Using Vector3.zero");
        startPosition = Vector3.zero;
        startRotationY = 0f;
    }

    // Method untuk membuat save baru (fresh save) di slot yang diberikan
    public static void CreateFreshSave(int slot, Vector3 position, float rotationY)
    {
        SaveSystem.CreateEmptySave(slot, position, rotationY);
    }
}
