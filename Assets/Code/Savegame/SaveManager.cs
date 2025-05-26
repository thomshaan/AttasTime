using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    private readonly string[] gameplayScenes = { "WorldMain", "House2" };

    public Inventory inventory;
    public PlayerStats playerStats;
    public GameObject player;
    public QuestManager questManager;
    public Transform playerSpawnPoint;

    public static int currentSaveSlot = 1;
    public static string spawnTargetID = "DefaultSpawn";

    private bool hasLoaded = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Simpan SaveManager antar scene
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (System.Array.Exists(gameplayScenes, name => name == scene.name))
        {
            if (!hasLoaded)
            {
                hasLoaded = true;

                // ⏬ Spawn Player dari Resources
                if (player == null)
                {
                    GameObject playerPrefab = Resources.Load<GameObject>("Atta");
                    if (playerPrefab != null)
                    {
                        player = Instantiate(playerPrefab);
                        Debug.Log("[SaveManager] Player spawned from prefab.");
                    }
                    else
                    {
                        Debug.LogError("Player prefab not found in Resources/Prefab folder.");
                        return;
                    }
                }

                // ⏬ Re-assign all references
                inventory = player.GetComponent<Inventory>();
                playerStats = player.GetComponent<PlayerStats>();
                questManager = FindObjectOfType<QuestManager>();
                playerSpawnPoint = GameObject.FindWithTag("PlayerSpawn")?.transform;

                if (inventory == null || playerStats == null || questManager == null)
                {
                    Debug.LogError("[SaveManager] LoadGame failed: references not assigned!");
                    return;
                }

                // ⏬ Load or start new game
                if (!SaveSystem.TryLoadPlayerStats(currentSaveSlot, out var data))
                    NewGame();
                else
                    LoadGame();
            }
        }
        else
        {
            Destroy(gameObject);
            Debug.Log("[SaveManager] Destroyed SaveManager on scene " + scene.name);
        }
    }

    public void LoadGame()
    {
        if (player == null || inventory == null || playerStats == null || questManager == null)
        {
            Debug.LogError("[SaveManager] LoadGame failed: references not assigned!");
            return;
        }

        // ⏬ Load inventory
        var simpleInventory = SaveSystem.LoadInventory(currentSaveSlot);
        inventory.LoadFromSimpleItemList(simpleInventory);

        // ⏬ Load player stats
        var stats = SaveSystem.LoadPlayerStats(currentSaveSlot);
        playerStats.SetStats(stats.coins, stats.xp);

        // ⏬ Load spawn position
        Vector3 spawnPos = stats.position;
        float spawnRotY = stats.rotY;

        if (!string.IsNullOrEmpty(spawnTargetID) && spawnTargetID != "DefaultSpawn")
        {
            SpawnPoint[] spawns = GameObject.FindObjectsOfType<SpawnPoint>();
            bool foundSpawn = false;
            foreach (var sp in spawns)
            {
                Debug.Log($"[SaveManager] Checking spawn point: {sp.spawnID}");
                if (sp.spawnID == spawnTargetID)
                {
                    spawnPos = sp.transform.position;
                    spawnRotY = sp.transform.eulerAngles.y;
                    foundSpawn = true;
                    Debug.Log($"[SaveManager] Spawn point matched: {spawnTargetID}");
                    break;
                }
            }
            if (!foundSpawn)
            {
                Debug.LogWarning($"[SaveManager] Spawn point with ID '{spawnTargetID}' not found. Using fallback spawn.");
            }
        }

        if (spawnPos == Vector3.zero && playerSpawnPoint != null)
        {
            spawnPos = playerSpawnPoint.position;
            spawnRotY = playerSpawnPoint.eulerAngles.y;
        }

        spawnPos.y += 1f; // offset sedikit dari tanah

        // ⏬ Reset physics (Rigidbody)
        var rb = player.GetComponent<Rigidbody>();
        if (rb != null && !rb.isKinematic)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // ⏬ Teleport player
        var controller = player.GetComponent<CharacterController>();
        if (controller != null)
        {
            controller.enabled = false;
            player.transform.position = spawnPos;
            player.transform.rotation = Quaternion.Euler(0, spawnRotY, 0);
            controller.enabled = true;
        }
        else
        {
            player.transform.position = spawnPos;
            player.transform.rotation = Quaternion.Euler(0, spawnRotY, 0);
        }

        // ⏬ Skip input drift bug
        var tpController = player.GetComponent<ThirdPersonController>();
        if (tpController != null)
        {
            tpController.SkipMovementNextFrame();
        }

        StartCoroutine(UpdateStatsDelayed(stats.coins, stats.xp));

        // ⏬ Quest and time
        var questData = SaveSystem.LoadQuests(currentSaveSlot);
        questManager.LoadQuestStateData(questData.questId, questData.questState);

        float timeOfDay = SaveSystem.LoadGameTime(currentSaveSlot);
        LightingManager.Instance.SetTimeOfDay(timeOfDay);

        Debug.Log($"[SaveManager] Game loaded from slot {currentSaveSlot} at spawn '{spawnTargetID}'");
    }

    private IEnumerator UpdateStatsDelayed(int coins, int xp)
    {
        yield return null;
        playerStats.SetStats(coins, xp);
    }

    public void NewGame()
    {
        Vector3 startPos = NewGameData.startPosition;
        float startRot = NewGameData.startRotationY;

        NewGameData.CreateFreshSave(currentSaveSlot, startPos, startRot);
        LoadGame();
    }

    public void SaveGame()
    {
        if (inventory == null || playerStats == null || player == null || questManager == null)
        {
            Debug.LogError("[SaveManager] SaveGame failed: references not assigned!");
            return;
        }

        SaveSystem.SaveInventory(inventory.ToSimpleItemList(), currentSaveSlot);
        SaveSystem.SavePlayerStats(
            playerStats.Coins,
            playerStats.XP,
            SceneManager.GetActiveScene().name,
            player.transform.position,
            player.transform.eulerAngles.y,
            currentSaveSlot
        );

        var (questId, questState) = questManager.GetQuestStateData();
        SaveSystem.SaveQuests(questId, questState, currentSaveSlot);

        SaveSystem.SaveGameTime(LightingManager.Instance.TimeOfDay, currentSaveSlot);

        Debug.Log($"[SaveManager] Game saved to slot {currentSaveSlot}");
        Debug.Log("[SaveManager] Player position saved: " + player.transform.position);
    }
}
