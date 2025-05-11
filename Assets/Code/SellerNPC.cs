using UnityEngine;

public class SellerNPC : MonoBehaviour, IInteractable
{
    private Inventory playerInventory;
    private PlayerStats playerStats;
    private Item itemForSale;
    private int stock = 0;

    public void InitializeSeller(Item item, int stockAmount)
    {
        itemForSale = item;
        stock = stockAmount;
    }

    public void ResetSeller(Item newItem, int newStock)
    {
        itemForSale = newItem;
        stock = newStock;
    }

    public void Interact()
    {
        if (itemForSale == null || stock <= 0) return;

        // ✅ Always find references safely
        playerInventory = FindObjectOfType<Inventory>();
        playerStats = FindObjectOfType<PlayerStats>(); GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        if (playerInventory == null)
            playerInventory = player.GetComponent<Inventory>();
        if (playerStats == null)
            playerStats = player.GetComponent<PlayerStats>();

        if (playerInventory == null)
        {
            Debug.LogWarning("⚠️ PlayerInventory not found.");
            return;
        }

        if (playerStats == null)
        {
            Debug.LogWarning("⚠️ PlayerStats not found.");
            return;
        }

        // ✅ Check coins
        if (playerStats.GetCoins() >= itemForSale.price)
        {
            playerStats.SpendCoins(itemForSale.price);
            playerInventory.SendMessage("AddItem", itemForSale);
            stock--;
            Debug.Log($"✅ Bought {itemForSale.name} for {itemForSale.price}. Stock left: {stock}");
        }
        else
        {
            Debug.Log("❌ Not enough coins!");
        }
    }

    public string GetInteractionPrompt()
    {
        return (itemForSale != null && stock > 0)
            ? $"Buy {itemForSale.name} ({stock} left) - {itemForSale.price} coins"
            : "Sold Out";
    }
}
