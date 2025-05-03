using UnityEngine;

public class SellerNPC : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject interactionUI;
    private Inventory playerInventory;
    private Item[] itemsForSale;
    private int currentIndex = 0;
    
    public void InitializeSeller(Item[] items)
    {
        itemsForSale = items;
        currentIndex = 0;

        if (interactionUI != null)
        {
            interactionUI.SetActive(false);
        }
    }

    public void Interact()
    {
        if (itemsForSale == null || currentIndex >= itemsForSale.Length) return;

        if (playerInventory == null)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) playerInventory = player.GetComponent<Inventory>();
        }

        if (playerInventory != null)
        {
            playerInventory.SendMessage("AddItem", itemsForSale[currentIndex]);
            Debug.Log($"Player bought: {itemsForSale[currentIndex].name}");
            currentIndex++;

            if (currentIndex >= itemsForSale.Length && interactionUI != null)
                interactionUI.SetActive(false);
        }
    }

    public string GetInteractionPrompt()
    {
        return (itemsForSale != null && currentIndex < itemsForSale.Length)
            ? $"Buy {itemsForSale[currentIndex].name}"
            : "Sold Out";
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && interactionUI != null && currentIndex < itemsForSale.Length)
        {
            interactionUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && interactionUI != null)
        {
            interactionUI.SetActive(false);
        }
    }
}
