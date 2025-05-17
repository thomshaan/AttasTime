using UnityEngine;

public class ItemGiver : MonoBehaviour, IInteractable
{
    [Header("Item to Give")]
    [SerializeField] private Item itemToGive;

    [Header("Optional")]
    [SerializeField] private GameObject interactionUI;

    private Inventory playerInventory;

    void Start()
    {
        if (interactionUI != null)
            interactionUI.SetActive(false);
    }

    public void Interact()
    {
        

        if (playerInventory == null)
        {
            playerInventory = FindObjectOfType<Inventory>();
            
        }

        if (playerInventory != null && itemToGive != null)
        {
            playerInventory.AddItem(itemToGive);
            

            if (interactionUI != null)
                interactionUI.SetActive(false);
        }
        else
        {
            
        }
    }

    public string GetInteractionPrompt()
    {
        return $"Take {itemToGive.name}";
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && interactionUI != null)
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
