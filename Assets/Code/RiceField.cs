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
    public GameObject riceItemPrefab;

    [Header("Spawn Points")]
    public Transform[] modelSpawnPoints;
    public Transform[] riceDropPoints;

    [Header("Item to Give")]
    public Item riceItem; // ScriptableObject

    private RiceFieldState currentState = RiceFieldState.ReadyToSeed;
    private float growthTimer = 0f;
    private bool isGrowing = false;
    private GameObject currentModel;
    private Inventory playerInventory;

    private void Update()
    {
        if (isGrowing)
        {
            growthTimer += Time.deltaTime;
            if (growthTimer >= growDurationInHours)
            {
                isGrowing = false;
                currentState = RiceFieldState.ReadyToHarvest;
                UpdateFieldModel(harvestPrefab);
                Debug.Log("Rice is ready to harvest!");
            }
        }
    }

    public string GetInteractionPrompt()
    {
        switch (currentState)
        {
            case RiceFieldState.ReadyToSeed: return "Plant Rice";
            case RiceFieldState.ReadyToHarvest: return "Harvest Rice";
            default: return null;
        }
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
        growthTimer = 0f;
        isGrowing = true;
        UpdateFieldModel(growPrefab);
        Debug.Log("Rice planted!");
    }

    private void HarvestRice()
    {
        foreach (Transform spawnPoint in riceDropPoints)
        {
            Instantiate(riceItemPrefab, spawnPoint.position, spawnPoint.rotation);
        }

        if (playerInventory != null && riceItem != null)
        {
            playerInventory.SendMessage("AddItem", riceItem);
            Debug.Log("Rice harvested and added to inventory!");
        }

        currentState = RiceFieldState.ReadyToSeed;
        UpdateFieldModel(null);
    }

    private void UpdateFieldModel(GameObject prefab)
    {
        // Destroy current models if any
        if (currentModel != null)
        {
            Destroy(currentModel);
        }

        // Remove all existing children of model spawn points
        foreach (Transform spawnPoint in modelSpawnPoints)
        {
            foreach (Transform child in spawnPoint)
            {
                Destroy(child.gameObject);
            }
        }

        // Spawn new models at all spawn points
        if (prefab != null)
        {
            foreach (Transform spawnPoint in modelSpawnPoints)
            {
                Instantiate(prefab, spawnPoint.position, spawnPoint.rotation, spawnPoint);
            }
        }
    }

}
