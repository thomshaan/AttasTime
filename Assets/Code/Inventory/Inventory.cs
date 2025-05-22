using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Collider))]
public class Inventory : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    InventoryUI ui;
    [SerializeField]
    AudioSource audioSource;

    [Header("Prefabs")]
    [SerializeField]
    GameObject droppedItemPrefab;

    [Header("Audio Clips")]
    [SerializeField]
    AudioClip pickUpItemAudio;
    [SerializeField]
    AudioClip dropItemAudio;

    [Header("State")]
    [SerializeField]
    SerializedDictionary<string, Item> inventory = new();

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DroppedItem"))
        {
            var droppedItem = other.GetComponent<DroppedItem>();
            if (droppedItem.pickedUp)
            {
                return;
            }
            droppedItem.pickedUp = true;
            AddItem(droppedItem.item);
            Destroy(other.gameObject);
            audioSource.PlayOneShot(pickUpItemAudio);
        }
    }

    public void AddItem(Item item)
    {
        var inventoryId = Guid.NewGuid().ToString();
        inventory.Add(inventoryId, item);
        ui.AddUIItem(inventoryId, item);
    }

    public void DropItem(string inventoryId)
    {
        var droppedItem = Instantiate(droppedItemPrefab, transform.position, Quaternion.identity).GetComponent<DroppedItem>();
        var item = inventory.GetValueOrDefault(inventoryId);
        droppedItem.Initialize(item);
        inventory.Remove(inventoryId);
        ui.RemoveUIItem(inventoryId);
        audioSource.PlayOneShot(dropItemAudio);
    }

    public bool Contains(Item item)
    {
        foreach (var pair in inventory)
        {
            if (pair.Value == item)
                return true;
        }
        return false;
    }

    public int CountOf(Item item)
    {
        int count = 0;
        foreach (var pair in inventory)
        {
            if (pair.Value == item)
            {
                count++;
            }
        }
        return count;
    }

    public List<SimpleItemSlot> ToSimpleItemList()
    {
        Dictionary<string, int> itemCountMap = new();

        foreach (var pair in inventory)
        {
            string id = pair.Value.id; // Assuming each Item has a unique 'id' field
            if (itemCountMap.ContainsKey(id))
                itemCountMap[id]++;
            else
                itemCountMap[id] = 1;
        }

        List<SimpleItemSlot> result = new();
        foreach (var kvp in itemCountMap)
        {
            result.Add(new SimpleItemSlot { itemId = kvp.Key, quantity = kvp.Value });
        }

        return result;
    }

    public void LoadFromSimpleItemList(List<SimpleItemSlot> itemList)
    {
        inventory.Clear();
        ui.ClearUI(); // Ensure this method clears the UI representation of the inventory

        foreach (var slot in itemList)
        {
            for (int i = 0; i < slot.quantity; i++)
            {
                // Retrieve the Item instance using its ID
                Item item = ItemDatabase.GetItemById(slot.itemId);
                AddItem(item);
            }
        }
    }


}
