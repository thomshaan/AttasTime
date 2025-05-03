using UnityEngine;
using System.Collections.Generic;

public class SellerSpawner : MonoBehaviour
{
    [Header("Spawn Points (Empty GameObjects)")]
    [SerializeField] private Transform[] spawnPoints;

    [Header("NPC Seller Prefabs (with different models)")]
    [SerializeField] private GameObject[] sellerPrefabs;

    [Header("Textures by Seller Type")]
    [SerializeField] private List<SellerTextureSet> textureSets;

    [Header("Item Pool (Premade Items)")]
    [SerializeField] private Item[] availableItems;

    void Start()
    {
        SpawnSellers();
    }

    void SpawnSellers()
    {
        int sellerCount = Mathf.Min(spawnPoints.Length, availableItems.Length / 2);

        for (int i = 0; i < sellerCount; i++)
        {
            // Randomly select a seller prefab
            GameObject prefab = sellerPrefabs[Random.Range(0, sellerPrefabs.Length)];
            GameObject seller = Instantiate(prefab, spawnPoints[i].position, Quaternion.identity);

            // Get SellerNPC script
            SellerNPC npc = seller.GetComponent<SellerNPC>();
            if (npc != null)
            {
                // Assign 2 items to this seller
                Item[] itemsToSell = new Item[2];
                itemsToSell[0] = availableItems[i * 2];
                itemsToSell[1] = availableItems[i * 2 + 1];
                npc.InitializeSeller(itemsToSell);

                // Apply random texture based on seller type
                SellerTypeIdentifier typeId = seller.GetComponent<SellerTypeIdentifier>();
                Renderer renderer = seller.GetComponentInChildren<Renderer>();

                if (typeId != null && renderer != null)
                {
                    Texture randomTexture = GetRandomTextureForType(typeId.sellerType);
                    if (randomTexture != null)
                    {
                        renderer.material.SetTexture("_BaseMap", randomTexture); // adjust property if using custom shader
                    }
                }
            }
            else
            {
                Debug.LogError("Seller prefab missing SellerNPC component!");
            }
        }
    }

    Texture GetRandomTextureForType(SellerType type)
    {
        foreach (var set in textureSets)
        {
            if (set.type == type && set.textures.Length > 0)
            {
                return set.textures[Random.Range(0, set.textures.Length)];
            }
        }
        return null;
    }
}
