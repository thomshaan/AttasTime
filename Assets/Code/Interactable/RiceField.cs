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

    private RiceFieldState currentState = RiceFieldState.ReadyToSeed;
    private bool isGrowing = false;
    private float growthStartTime;
    private GameObject currentModel;
    private Inventory playerInventory;

    private void Update()
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
                Debug.Log("🌾 Rice is ready to harvest!");
            }
        }
    }

    private float GetElapsedGameHours(float start, float now)
    {
        // Handles day rollover (e.g. from 23 to 1)
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
            if (player != null) playerInventory = player.GetComponent<Inventory>();
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
        if (playerInventory != null && riceItem != null)
        {
            playerInventory.AddItem(riceItem);
            Debug.Log("✅ Rice harvested and added directly to inventory!");
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
