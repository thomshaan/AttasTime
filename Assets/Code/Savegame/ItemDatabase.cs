using System.Collections.Generic;
using UnityEngine;

public static class ItemDatabase
{
    private static Dictionary<string, Item> itemDict;

    static ItemDatabase()
    {
        itemDict = new Dictionary<string, Item>();
        foreach (var item in Resources.LoadAll<Item>("Items"))
        {
            if (!itemDict.ContainsKey(item.id))
            {
                itemDict[item.id] = item;
            }
        }
    }

    public static Item GetItemById(string id)
    {
        if (itemDict.TryGetValue(id, out Item item))
        {
            return item;
        }

        Debug.LogWarning($"[ItemDatabase] Item with ID '{id}' not found!");
        return null;
    }
}
