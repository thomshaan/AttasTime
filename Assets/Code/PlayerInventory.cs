using UnityEngine;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour
{
    [Header("Player Stats")]
    public int currency = 100;

    [Header("Inventory")]
    public Dictionary<CommodityType, int> commodities = new Dictionary<CommodityType, int>();

    private void Start()
    {
        // Initialize all commodity types to 0
        foreach (CommodityType type in System.Enum.GetValues(typeof(CommodityType)))
        {
            commodities[type] = 0;
        }
    }

    public void AddCommodity(CommodityType type, int amount)
    {
        if (commodities.ContainsKey(type))
        {
            commodities[type] += amount;
        }
        else
        {
            commodities[type] = amount;
        }

        Debug.Log($"You now have {commodities[type]} of {type}");
    }

    public void RemoveCommodity(CommodityType type, int amount)
    {
        if (commodities.ContainsKey(type) && commodities[type] >= amount)
        {
            commodities[type] -= amount;
            Debug.Log($"Removed {amount} of {type}. Now you have {commodities[type]} left.");
        }
        else
        {
            Debug.LogWarning($"Not enough {type} to remove.");
        }
    }

    public int GetCommodityCount(CommodityType type)
    {
        return commodities.ContainsKey(type) ? commodities[type] : 0;
    }
}
