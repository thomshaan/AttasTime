using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;

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
        if (scene.name == "WorldMain" && !hasLoaded)
        {
            hasLoaded = true;

            if (!SaveSystem.TryLoadPlayerStats(currentSaveSlot, out var data))
            {
                Debug.Log("[SaveManager] No save found, creating fresh save...");
                NewGame();
            }
            else
            {
                Debug.Log("[SaveManager] Save found, loading...");
                LoadGame();
            }
        }
    }

    // Kosongkan Start supaya tidak ada load game ganda
    void Start()
    {
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) // Debug: force reset posisi
        {
            player.transform.position = playerSpawnPoint.position + Vector3.up;
            Debug.Log("[SaveManager] Forced player position to spawn point");
        }
    }

    public void SaveGame()
    {
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
        if (player == null)
        {
            Debug.LogError("[SaveManager] Player reference is null!");
            return;
        }

        // Load Inventory
        List<SimpleItemSlot> simpleInventory = SaveSystem.LoadInventory(currentSaveSlot);
        inventory.LoadFromSimpleItemList(simpleInventory);

        // Load Player Stats (including saved position)
        var stats = SaveSystem.LoadPlayerStats(currentSaveSlot);
        playerStats.coins = stats.coins;
        playerStats.xp = stats.xp;

        // Determine spawn position: saved pos if valid, else fallback spawn point
        Vector3 spawnPos = stats.position;
        if (spawnPos == Vector3.zero && playerSpawnPoint != null)
        {
            spawnPos = playerSpawnPoint.position;
            Debug.LogWarning("[SaveManager] Using fallback spawn point position.");
        }

        // Raise Y slightly to avoid falling through ground on spawn
        spawnPos.y += 1.0f;

        Debug.Log($"[SaveManager] Spawn position used: {spawnPos}");

        // Reset Rigidbody velocity if exists and non-kinematic
        var rb = player.GetComponent<Rigidbody>();
        if (rb != null && !rb.isKinematic)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // Move player safely using CharacterController if present
        var controller = player.GetComponent<CharacterController>();
        if (controller != null)
        {
            controller.enabled = false; // disable to prevent physics conflicts
            player.transform.position = spawnPos;
            player.transform.rotation = Quaternion.Euler(0, stats.rotY, 0);
            controller.enabled = true;  // re-enable after repositioning
        }
        else
        {
            // No CharacterController - set transform directly
            player.transform.position = spawnPos;
            player.transform.rotation = Quaternion.Euler(0, stats.rotY, 0);
        }
        StartCoroutine(UpdateStatsDelayed(stats.coins, stats.xp));
        // Load Quest State
        var questData = SaveSystem.LoadQuests(currentSaveSlot);
        questManager.LoadQuestStateData(questData.questId, questData.questState);

        // Load Time
        float timeOfDay = SaveSystem.LoadGameTime(currentSaveSlot);
        LightingManager.Instance.SetTimeOfDay(timeOfDay);

        Debug.Log($"[SaveManager] Game loaded from slot {currentSaveSlot}");
    }

    private IEnumerator UpdateStatsDelayed(int coins, int xp)
    {
        yield return null; // tunda 1 frame supaya UI siap
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
