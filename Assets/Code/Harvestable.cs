using UnityEngine;

public class HarvestableObject : MonoBehaviour
{
    public enum HarvestState { Empty, Growing, ReadyToHarvest }
    public HarvestState currentState = HarvestState.Empty;

    [Header("Harvest Settings")]
    public CommodityType yieldCommodity;         // What item it gives
    public int yieldAmount = 1;                  // How much it gives
    public float growDurationHours = 48f;        // In-game time to grow

    private float growTimer = 0f;
    private float lastRecordedTime;

    [Header("Visuals")]
    public GameObject seedVisual;
    public GameObject grownVisual;

    private LightingManager timeManager;

    void Start()
    {
        timeManager = LightingManager.Instance;
        lastRecordedTime = timeManager.TimeOfDay;
        UpdateVisual();
    }

    void Update()
    {
        if (currentState == HarvestState.Growing)
        {
            float currentTime = timeManager.TimeOfDay;
            float deltaTime = currentTime - lastRecordedTime;
            if (deltaTime < 0) deltaTime += 24f; // wrap around midnight

            growTimer += deltaTime;
            lastRecordedTime = currentTime;

            if (growTimer >= growDurationHours)
            {
                currentState = HarvestState.ReadyToHarvest;
                UpdateVisual();
            }
        }
    }

    void UpdateVisual()
    {
        if (seedVisual) seedVisual.SetActive(currentState == HarvestState.Growing);
        if (grownVisual) grownVisual.SetActive(currentState == HarvestState.ReadyToHarvest);
    }

    public void Interact(PlayerInventory inventory)
    {
        switch (currentState)
        {
            case HarvestState.Empty:
                currentState = HarvestState.Growing;
                growTimer = 0f;
                lastRecordedTime = timeManager.TimeOfDay;
                UpdateVisual();
                break;

            case HarvestState.ReadyToHarvest:
                inventory.AddCommodity(yieldCommodity, yieldAmount);
                currentState = HarvestState.Empty;
                growTimer = 0f;
                UpdateVisual();
                break;
        }
    }
}
