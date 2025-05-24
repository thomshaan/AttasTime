using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    public Inventory inventory;
    public PlayerStats playerStats;
    public GameObject player;
    public QuestManager questManager;
    public Transform playerSpawnPoint; // fallback spawn point for new game

    public static int currentSaveSlot = 1;

    private bool hasLoaded = false;

    private void Awake()
    {
        // Singleton pattern supaya hanya ada 1 instance SaveManager
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "WorldMain" || scene.name == "HouseInterior")
        {
            // Gameplay scene logic
            if (!hasLoaded)
            {
                hasLoaded = true;
                if (!SaveSystem.TryLoadPlayerStats(currentSaveSlot, out var data))
                    NewGame();
                else
                    LoadGame();
            }
        }
        else if (scene.name == "Mainmenu")  // Sesuaikan dengan case nama scene di Unity
        {
            Destroy(gameObject);
            Debug.Log("[SaveManager] Destroyed SaveManager on scene " + scene.name);
        }
        else
        {
            // Scene lain selain gameplay dan mainmenu
            Destroy(gameObject);
            Debug.Log("[SaveManager] Destroyed SaveManager on scene " + scene.name);
        }
    }


    void Start() { }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (player != null && playerSpawnPoint != null)
            {
                player.transform.position = playerSpawnPoint.position + Vector3.up;
                Debug.Log("[SaveManager] Forced player position to spawn point");
            }
        }
    }

    public void SaveGame()
    {
        if (inventory == null || playerStats == null || player == null || questManager == null)
        {
            Debug.LogError("[SaveManager] SaveGame failed: references not assigned!");
            return;
        }

        List<SimpleItemSlot> simpleInventory = inventory.ToSimpleItemList();
        SaveSystem.SaveInventory(simpleInventory, currentSaveSlot);

        SaveSystem.SavePlayerStats(
            playerStats.coins,
            playerStats.xp,
            SceneManager.GetActiveScene().name,
            player.transform.position,
            player.transform.eulerAngles.y,
            currentSaveSlot
        );

        (string questId, string questState) = questManager.GetQuestStateData();
        SaveSystem.SaveQuests(questId, questState, currentSaveSlot);

        float time = LightingManager.Instance.TimeOfDay;
        SaveSystem.SaveGameTime(time, currentSaveSlot);

        Debug.Log($"[SaveManager] Game saved to slot {currentSaveSlot}");
        Debug.Log("[SaveManager] Player position saved: " + player.transform.position);
    }

    public void LoadGame()
    {
        if (player == null || inventory == null || playerStats == null || questManager == null || playerSpawnPoint == null)
        {
            Debug.LogError("[SaveManager] LoadGame failed: references not assigned!");
            return;
        }

        List<SimpleItemSlot> simpleInventory = SaveSystem.LoadInventory(currentSaveSlot);
        inventory.LoadFromSimpleItemList(simpleInventory);

        var stats = SaveSystem.LoadPlayerStats(currentSaveSlot);
        playerStats.coins = stats.coins;
        playerStats.xp = stats.xp;

        Vector3 spawnPos = stats.position;
        if (spawnPos == Vector3.zero)
        {
            spawnPos = playerSpawnPoint.position;
            Debug.LogWarning("[SaveManager] Using fallback spawn point position.");
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
            player.transform.rotation = Quaternion.Euler(0, stats.rotY, 0);
            controller.enabled = true;
        }
        else
        {
            player.transform.position = spawnPos;
            player.transform.rotation = Quaternion.Euler(0, stats.rotY, 0);
        }

        StartCoroutine(UpdateStatsDelayed(stats.coins, stats.xp));

        var questData = SaveSystem.LoadQuests(currentSaveSlot);
        questManager.LoadQuestStateData(questData.questId, questData.questState);

        float timeOfDay = SaveSystem.LoadGameTime(currentSaveSlot);
        LightingManager.Instance.SetTimeOfDay(timeOfDay);

        Debug.Log($"[SaveManager] Game loaded from slot {currentSaveSlot}");
    }

    private IEnumerator UpdateStatsDelayed(int coins, int xp)
    {
        yield return null; // delay satu frame supaya UI siap update
        playerStats.SetStats(coins, xp);
    }

    public void NewGame()
    {
        Vector3 startPos = NewGameData.startPosition;
        float startRot = NewGameData.startRotationY;

        NewGameData.CreateFreshSave(currentSaveSlot, startPos, startRot);

        LoadGame();
    }
}
