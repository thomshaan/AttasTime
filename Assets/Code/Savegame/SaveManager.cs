using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    private readonly string[] gameplayScenes = { "WorldMain", "House2", "RumahGadang" };

    public Inventory inventory;
    public PlayerStats playerStats;
    public GameObject player;
    public QuestManager questManager;
    public Transform playerSpawnPoint;
    private int loadGameCallCount = 0;

    public static int currentSaveSlot = 1;
    public static string spawnTargetID = "DefaultSpawn"; // default spawn ID

    private bool hasLoaded = false;
    public static bool isSceneTriggerSpawn = false;

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

                inventory = player.GetComponent<Inventory>();
                playerStats = player.GetComponent<PlayerStats>();
                questManager = FindObjectOfType<QuestManager>();
                playerSpawnPoint = GameObject.FindWithTag("PlayerSpawn")?.transform;

                if (inventory == null || playerStats == null || questManager == null)
                {
                    Debug.LogError("[SaveManager] LoadGame failed: references not assigned!");
                    return;
                }

                if (!SaveSystem.TryLoadPlayerStats(currentSaveSlot, out var data))
                {
                    spawnTargetID = "SpawnRumah";
                    Debug.Log("[SaveManager] No existing save found, using default spawn: SpawnRumah");
                }
                else
                {
                    if (!isSceneTriggerSpawn)
                    {
                        spawnTargetID = data.spawnTargetID;
                        Debug.Log("[SaveManager] Loaded spawnTargetID from DB: " + spawnTargetID);
                    }
                    else
                    {
                        Debug.Log("[SaveManager] Using spawnTargetID from SceneTrigger: " + spawnTargetID);
                    }
                }

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
        loadGameCallCount++;
        Debug.Log($"[SaveManager] LoadGame called #{loadGameCallCount}");
        if (loadGameCallCount > 1)
        {
            Debug.LogWarning("[SaveManager] LoadGame called more than once! Aborting redundant call.");
            return;
        }
        if (player == null || inventory == null || playerStats == null || questManager == null)
        {
            Debug.LogError("[SaveManager] LoadGame failed: references not assigned!");
            return;
        }

        var simpleInventory = SaveSystem.LoadInventory(currentSaveSlot);
        inventory.LoadFromSimpleItemList(simpleInventory);

        var stats = SaveSystem.LoadPlayerStats(currentSaveSlot);
        playerStats.SetStats(stats.coins, stats.xp);

        string currentScene = SceneManager.GetActiveScene().name;

        Debug.Log($"[SaveManager] LoadGame start - spawnTargetID: {spawnTargetID}, isSceneTriggerSpawn: {isSceneTriggerSpawn}");

        Vector3 spawnPos = Vector3.zero;
        float spawnRotY = 0f;
        bool foundSpawnPoint = false;

        if (currentScene == "RumahGadang")
        {
            SpawnPoint[] spawns = GameObject.FindObjectsOfType<SpawnPoint>();
            foreach (var sp in spawns)
            {
                if (sp.spawnID == "SpawnGadang")
                {
                    spawnPos = sp.transform.position;
                    spawnRotY = sp.transform.eulerAngles.y;
                    foundSpawnPoint = true;
                    Debug.Log("[SaveManager] Spawn point set to SpawnGadang for scene RumahGadang");
                    break;
                }
            }
        }
        else if (currentScene == "House2")
        {
            SpawnPoint[] spawns = GameObject.FindObjectsOfType<SpawnPoint>();
            foreach (var sp in spawns)
            {
                if (sp.spawnID == "SpawnInterior")
                {
                    spawnPos = sp.transform.position;
                    spawnRotY = sp.transform.eulerAngles.y;
                    foundSpawnPoint = true;
                    Debug.Log("[SaveManager] Spawn point set to SpawnInterior for scene House2");
                    break;
                }
            }
        }
        else if (isSceneTriggerSpawn)
        {
            if (!string.IsNullOrEmpty(spawnTargetID) && spawnTargetID != "DefaultSpawn")
            {
                SpawnPoint[] spawns = GameObject.FindObjectsOfType<SpawnPoint>();
                foreach (var sp in spawns)
                {
                    if (sp.spawnID == spawnTargetID)
                    {
                        spawnPos = sp.transform.position;
                        spawnRotY = sp.transform.eulerAngles.y;
                        foundSpawnPoint = true;
                        Debug.Log("[SaveManager] Spawn point set by SceneTrigger: " + spawnTargetID);
                        break;
                    }
                }
            }
            // jangan reset flag disini
        }
        else if (stats.position != Vector3.zero && currentScene == stats.sceneName)
        {
            spawnPos = stats.position;
            spawnRotY = stats.rotY;
            foundSpawnPoint = true;
            Debug.Log("[SaveManager] Spawn position from saved data used");
        }
        else
        {
            if (!string.IsNullOrEmpty(spawnTargetID) && spawnTargetID != "DefaultSpawn")
            {
                SpawnPoint[] spawns = GameObject.FindObjectsOfType<SpawnPoint>();
                foreach (var sp in spawns)
                {
                    if (sp.spawnID == spawnTargetID)
                    {
                        spawnPos = sp.transform.position;
                        spawnRotY = sp.transform.eulerAngles.y;
                        foundSpawnPoint = true;
                        Debug.Log("[SaveManager] Spawn point set by spawnTargetID: " + spawnTargetID);
                        break;
                    }
                }
            }
        }

        if (!foundSpawnPoint && playerSpawnPoint != null)
        {
            spawnPos = playerSpawnPoint.position;
            spawnRotY = playerSpawnPoint.eulerAngles.y;
            Debug.Log("[SaveManager] Spawn point fallback to default PlayerSpawn");
        }

        spawnPos.y += 1f;

        var rb = player.GetComponent<Rigidbody>();
        if (rb != null && !rb.isKinematic)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

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

        var tpController = player.GetComponent<ThirdPersonController>();
        if (tpController != null)
        {
            tpController.SkipMovementNextFrame();
        }

        if (isSceneTriggerSpawn)
        {
            isSceneTriggerSpawn = false;
            Debug.Log("[SaveManager] isSceneTriggerSpawn flag reset");
        }

        var questData = SaveSystem.LoadQuests(currentSaveSlot);
        questManager.LoadQuestStateData(questData.questId, questData.questState);

        float timeOfDay = SaveSystem.LoadGameTime(currentSaveSlot);
        LightingManager.Instance.SetTimeOfDay(timeOfDay);

        Debug.Log($"[SaveManager] Game loaded from slot {currentSaveSlot} at scene {currentScene}");
    }

    private IEnumerator UpdateStatsDelayed(int coins, int xp)
    {
        yield return null;
        playerStats.SetStats(coins, xp);
    }

    public void NewGame()
    {
        NewGameData.InitializeStartPosition("SpawnRumah");
        spawnTargetID = "SpawnRumah";

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
            spawnTargetID
        );

        var (questId, questState) = questManager.GetQuestStateData();
        SaveSystem.SaveQuests(questId, questState, currentSaveSlot);

        SaveSystem.SaveGameTime(LightingManager.Instance.TimeOfDay, currentSaveSlot);

        Debug.Log($"[SaveManager] Game saved to slot {currentSaveSlot}");
        Debug.Log("[SaveManager] Player position saved: " + player.transform.position);
    }

    public void DeleteSaveSlot(int slot)
    {
        Debug.Log($"[SaveManager] Deleting save slot {slot}");

        spawnTargetID = "SpawnRumah";

        NewGameData.InitializeStartPosition(spawnTargetID);

        NewGameData.CreateFreshSave(slot, NewGameData.startPosition, NewGameData.startRotationY);

        if (slot == currentSaveSlot)
        {
            LoadGame();
        }
    }
}
