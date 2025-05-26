using UnityEngine;

public enum RiceFieldState
{
    ReadyToSeed,
    Growing,
    ReadyToHarvest
}

public class RiceField : MonoBehaviour, IInteractable
{
    [Header("Growth Timing (in game hours)")]
    public float growDurationInHours = 48f;

    [Header("Prefabs")]
    public GameObject seedPrefab;
    public GameObject growPrefab;
    public GameObject harvestPrefab;

    [Header("Spawn Points")]
    public Transform[] modelSpawnPoints;

    [Header("Item to Give")]
    public Item riceItem;

    [Header("Inventory References")]
    public Inventory inventory; // Optional static assignment
    private Inventory playerInventory;

    private RiceFieldState currentState = RiceFieldState.ReadyToSeed;
    private bool isGrowing = false;
    private float growthStartTime;
    private GameObject currentModel;
    private FloatingIconController iconController;


    void Awake()
    {
        // Auto-assign fallback for inventory field
        if (inventory == null)
        {
            inventory = FindObjectOfType<Inventory>();
            Debug.Log("[RiceField] Auto-assigned 'inventory': " + inventory);
        }
        iconController = gameObject.AddComponent<FloatingIconController>();
        UpdateFloatingIcon();
    }

    void Update()
    {
        if (isGrowing && LightingManager.Instance != null)
        {
            float timeNow = LightingManager.Instance.TimeOfDay;
            float elapsedHours = GetElapsedGameHours(growthStartTime, timeNow);

            if (elapsedHours >= growDurationInHours)
            {
                isGrowing = false;
                currentState = RiceFieldState.ReadyToHarvest;
                UpdateFieldModel(harvestPrefab);
                Debug.Log(riceItem.name + " is ready to harvest!");
            }
        }
    }

    private void UpdateFloatingIcon()
    {
        if (currentState == RiceFieldState.ReadyToSeed || currentState == RiceFieldState.ReadyToHarvest)
        {
            iconController.ShowIconFromItem(riceItem, transform);
        }
        else
        {
            iconController.HideIcon();
        }
    }

    private float GetElapsedGameHours(float start, float now)
    {
        if (now >= start)
            return now - start;
        else
            return (24 - start) + now;
    }

    public string GetInteractionPrompt()
    {
        return currentState switch
        {
            RiceFieldState.ReadyToSeed => "Plant Rice",
            RiceFieldState.ReadyToHarvest => "Harvest Rice",
            _ => null
        };
    }

    public void Interact()
    {
        if (playerInventory == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerInventory = player.GetComponent<Inventory>();
                Debug.Log("[Interact] Found Player: " + player.name);
                Debug.Log("[Interact] Assigned playerInventory: " + playerInventory);
            }
        }

        switch (currentState)
        {
            case RiceFieldState.ReadyToSeed:
                PlantSeed();
                break;

            case RiceFieldState.ReadyToHarvest:
                HarvestRice();
                break;
        }
    }

    private void PlantSeed()
    {
        currentState = RiceFieldState.Growing;
        growthStartTime = LightingManager.Instance.TimeOfDay;
        isGrowing = true;
        UpdateFieldModel(growPrefab);
        Debug.Log("🌱 Rice planted!");
    }

    private void HarvestRice()
    {
        if (playerInventory == null)
        {
            playerInventory = FindObjectOfType<Inventory>();
            Debug.Log("[Harvest] Auto-reassigned Inventory: " + playerInventory);
        }

        if (playerInventory != null && riceItem != null)
        {
            playerInventory.AddItem(riceItem);
            Debug.Log("✅ Rice harvested: " + riceItem.name);
        }
        else
        {
            Debug.LogWarning("❌ Inventory or Rice Item is missing.");
        }

        currentState = RiceFieldState.ReadyToSeed;
        UpdateFieldModel(null);
    }

    private void UpdateFieldModel(GameObject prefab)
    {
        if (currentModel != null)
        {
            Destroy(currentModel);
        }

        foreach (Transform spawnPoint in modelSpawnPoints)
        {
            foreach (Transform child in spawnPoint)
            {
                Destroy(child.gameObject);
            }
        }

        if (prefab != null)
        {
            foreach (Transform spawnPoint in modelSpawnPoints)
            {
                Instantiate(prefab, spawnPoint.position, spawnPoint.rotation, spawnPoint);
            }
        }
    }
}
