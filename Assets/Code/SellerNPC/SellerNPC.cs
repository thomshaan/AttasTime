using UnityEngine;

public class SellerNPC : MonoBehaviour, IInteractable
{
    private Item item;
    private int stock;
    private Inventory inventory;
    public PlayerStats playerStats;
    public GameObject iconUIPrefab;
    private SellerIconUI iconUI;

    void Awake()
    {
        inventory = FindObjectOfType<Inventory>();
        if (inventory != null)
            Debug.Log("[SellerNPC] Inventory found: " + inventory.name);
        else
            Debug.LogWarning("[SellerNPC] ❌ Inventory not found at Awake.");
        playerStats = FindObjectOfType<PlayerStats>();
        if (playerStats != null)
            Debug.Log("[SellerNPC] Found PlayerStats: " + playerStats.name);
        else
            Debug.LogWarning("[SellerNPC] ❌ PlayerStats not found.");
    }

    public void InitializeSeller(Item newItem, int newStock)
    {
        item = newItem;
        stock = newStock;

        if (item != null && item.icon != null)
        {
            GameObject prefab = Resources.Load<GameObject>("UI/SellerIconUI");
            if (prefab != null)
            {
                GameObject uiInstance = Instantiate(prefab);
                iconUI = uiInstance.GetComponent<SellerIconUI>();
                iconUI.Initialize(transform, item.icon);
            }
            else
            {
                Debug.LogWarning("[SellerNPC] ❌ SellerIconUI prefab not found in Resources/UI.");
            }
        }
    }

    public void Interact()
    {
        Debug.Log("[SellerNPC] Interact called");

        if (inventory == null)
            inventory = FindObjectOfType<Inventory>();

        if (playerStats == null)
            playerStats = FindObjectOfType<PlayerStats>();

        if (item == null)
        {
            Debug.LogWarning("[SellerNPC] ❌ Item is null.");
            return;
        }

        if (stock <= 0)
        {
            Debug.LogWarning("[SellerNPC] ❌ Out of stock.");
            return;
        }

        if (inventory == null || playerStats == null)
        {
            Debug.LogWarning("[SellerNPC] ❌ Missing Inventory or PlayerStats.");
            return;
        }

        if (playerStats.coins < item.price)
        {
            Debug.LogWarning($"[SellerNPC] ❌ Not enough coins. Need {item.price}, have {playerStats.coins}");
            return;
        }

        // Perform transaction
        bool success = playerStats.SpendCoins(item.price);

        if (success)
        {
            inventory.AddItem(item);
            stock--;
            Debug.Log($"[SellerNPC] ✅ Sold {item.name} for {item.price} coins. Remaining: {playerStats.coins}");
        }
        inventory.AddItem(item);
        stock--;

        Debug.Log($"[SellerNPC] ✅ Sold {item.name} for {item.price} coins. Remaining coins: {playerStats.coins}");
    }

    public string GetInteractionPrompt()
    {
        return item != null ? $"Buy {item.name} ({stock} left)" : "Seller";
    }
}
