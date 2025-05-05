using UnityEngine;

public class SellerNPC : MonoBehaviour, IInteractable
{
    private Inventory playerInventory;
    private Item itemForSale;
    private int stock = 0;

    public void InitializeSeller(Item item, int stockAmount)
    {
        itemForSale = item;
        stock = stockAmount;
    }

    public void Interact()
    {
        if (itemForSale == null || stock <= 0) return;

        if (playerInventory == null)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) playerInventory = player.GetComponent<Inventory>();
        }

        if (playerInventory != null)
        {
            playerInventory.SendMessage("AddItem", itemForSale);
            Debug.Log($"Player bought: {itemForSale.name} (Stock left: {stock - 1})");
            stock--;
        }
    }

    public string GetInteractionPrompt()
    {
        return (itemForSale != null && stock > 0)
            ? $"Buy {itemForSale.name} ({stock} left)"
            : "Sold Out";
    }
}
