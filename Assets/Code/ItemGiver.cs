using UnityEngine;

public class ItemGiver : MonoBehaviour
{
    [Header("Item to Give")]
    [SerializeField] private Item itemToGive;

    [Header("Optional")]
    [SerializeField] private GameObject interactionUI; // "Press E" or custom button

    private bool playerInRange = false;
    private Inventory playerInventory;

    void Update()
    {
        // Desktop / keyboard input
        if (playerInRange && Input.GetButtonDown("Submit")) // Change to "Fire1" or custom if needed
        {
            GiveItem();
        }
    }

    public void GiveItem() // Can also be called from a mobile UI button
    {
        if (playerInventory != null && itemToGive != null)
        {
            playerInventory.SendMessage("AddItem", itemToGive);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInventory = other.GetComponent<Inventory>();
            playerInRange = true;
            if (interactionUI != null) interactionUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            playerInventory = null;
            if (interactionUI != null) interactionUI.SetActive(false);
        }
    }
}
