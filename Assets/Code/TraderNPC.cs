using UnityEngine;

public class TraderNPC : MonoBehaviour
{
    [Header("Trader Settings")]
    public CommodityType commodityType;
    public int stock = 10;
    public int price = 5; // basic price, you can expand later

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Try to get the PlayerInventory component
            PlayerInventory playerInventory = other.GetComponent<PlayerInventory>();

            if (playerInventory != null)
            {
                Debug.Log($"You are now talking to a trader of {commodityType}");

                // Example: player buys 1 unit of commodity
                if (playerInventory.currency >= price)
                {
                    playerInventory.currency -= price;
                    playerInventory.AddCommodity(commodityType, 1);
                    stock--;
                    Debug.Log($"Bought 1 {commodityType}. Remaining stock: {stock}");
                }
                else
                {
                    Debug.Log("Not enough money to buy.");
                }
            }
            else
            {
                Debug.LogWarning("PlayerInventory script not found on player!");
            }
        }
    }

    public void OpenTradeUI()
    {
        // Later replace this with actual UI
        Debug.Log($"Opening trade UI for commodity: {commodityType}");
    }

    public void AutomaticBuy()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        PlayerInventory inventory = player.GetComponent<PlayerInventory>();

        if (inventory == null)
        {
            Debug.LogWarning("PlayerInventory script not found on player!");
            return;
        }

        if (inventory.currency >= price)
        {
            inventory.currency -= price;
            inventory.AddCommodity(commodityType, 1);
            Debug.Log($"Bought 1 {commodityType} for {price} coins.");
        }
        else
        {
            Debug.Log("Not enough currency to buy.");
        }
    }

}
