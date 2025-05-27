using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

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
    public static string spawnTargetID = "DefaultSpawn"; // default spawn ID

    private bool hasLoaded = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Keep SaveManager across scenes
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

                // Spawn player prefab if not exists
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

                // Re-assign references
                inventory = player.GetComponent<Inventory>();
                playerStats = player.GetComponent<PlayerStats>();
                questManager = FindObjectOfType<QuestManager>();
                playerSpawnPoint = GameObject.FindWithTag("PlayerSpawn")?.transform;

                if (inventory == null || playerStats == null || questManager == null)
                {
                    Debug.LogError("[SaveManager] LoadGame failed: references not assigned!");
                    return;
                }

                // Try load player stats from DB
                if (!SaveSystem.TryLoadPlayerStats(currentSaveSlot, out var data))
                {
                    // No existing save: create new save at SpawnRumah
                    Debug.Log("[SaveManager] No existing save found, creating new save.");

                    NewGameData.InitializeStartPosition("SpawnRumah");
                    spawnTargetID = "SpawnRumah";

                    NewGameData.CreateFreshSave(currentSaveSlot, NewGameData.startPosition, NewGameData.startRotationY);
                    LoadGame();
                }
                else
                {
                    // Existing save found: assign spawnTargetID loaded from DB
                    spawnTargetID = data.spawnTargetID;
                    Debug.Log("[SaveManager] Loaded spawnTargetID: " + spawnTargetID);
                    LoadGame();
                }
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

        // Load inventory
        var simpleInventory = SaveSystem.LoadInventory(currentSaveSlot);
        inventory.LoadFromSimpleItemList(simpleInventory);

        // Load player stats
        var stats = SaveSystem.LoadPlayerStats(currentSaveSlot);
        playerStats.SetStats(stats.coins, stats.xp);

        // Determine spawn position & rotation
        Vector3 spawnPos = stats.position;
        float spawnRotY = stats.rotY;

        bool useSpawnPointPosition = false;

        // Use spawnTargetID to find the correct spawn point
        if (!string.IsNullOrEmpty(spawnTargetID) && spawnTargetID != "DefaultSpawn")
        {
            // Jika posisi yang disimpan nol, pakai spawn point posisi
            if (spawnPos == Vector3.zero)
            {
                SpawnPoint[] spawns = GameObject.FindObjectsOfType<SpawnPoint>();
                foreach (var sp in spawns)
                {
                    if (sp.spawnID == spawnTargetID)
                    {
                        spawnPos = sp.transform.position;
                        spawnRotY = sp.transform.eulerAngles.y;
                        useSpawnPointPosition = true;
                        Debug.Log("[SaveManager] Using spawn point position for spawnTargetID: " + spawnTargetID);
                        break;
                    }
                }
            }
        }

        // Fallback to default spawn point in scene if position is zero
        if (spawnPos == Vector3.zero && !useSpawnPointPosition && playerSpawnPoint != null)
        {
            spawnPos = playerSpawnPoint.position;
            spawnRotY = playerSpawnPoint.eulerAngles.y;
        }

        spawnPos.y += 1f; // Offset above ground

        // Reset physics velocity to avoid glitches
        var rb = player.GetComponent<Rigidbody>();
        if (rb != null && !rb.isKinematic)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // Teleport player to spawn position
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

        // Fix input drift next frame for smooth movement
        var tpController = player.GetComponent<ThirdPersonController>();
        if (tpController != null)
        {
            tpController.SkipMovementNextFrame();
        }

        StartCoroutine(UpdateStatsDelayed(stats.coins, stats.xp));

        // Load quests
        var questData = SaveSystem.LoadQuests(currentSaveSlot);
        questManager.LoadQuestStateData(questData.questId, questData.questState);

        // Load time of day
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
        NewGameData.InitializeStartPosition("SpawnRumah");
        spawnTargetID = "SpawnRumah";  // Always set spawnTargetID when new game starts

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
            currentSaveSlot,
            spawnTargetID  // Save current spawnTargetID to DB
        );

        var (questId, questState) = questManager.GetQuestStateData();
        SaveSystem.SaveQuests(questId, questState, currentSaveSlot);

        SaveSystem.SaveGameTime(LightingManager.Instance.TimeOfDay, currentSaveSlot);

        Debug.Log($"[SaveManager] Game saved to slot {currentSaveSlot}");
        Debug.Log("[SaveManager] Player position saved: " + player.transform.position);
    }
}
