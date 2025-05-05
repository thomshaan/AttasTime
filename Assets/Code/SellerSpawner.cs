using UnityEngine;
using System.Collections.Generic;

public class SellerSpawner : MonoBehaviour
{
    [Header("Spawn Points (8 Empty GameObjects)")]
    [SerializeField] private Transform[] spawnPoints;

    [Header("Seller Prefabs (different models)")]
    [SerializeField] private GameObject[] sellerPrefabs;

    [Header("Textures by Seller Type")]
    [SerializeField] private List<SellerTextureSet> textureSets;

    [Header("Item Pool (premade)")]
    [SerializeField] private Item[] availableItems;

    void Start()
    {
        SpawnSellers();
    }

    void SpawnSellers()
    {
        int sellerCount = spawnPoints.Length;

        for (int i = 0; i < sellerCount; i++)
        {
            GameObject prefab = sellerPrefabs[Random.Range(0, sellerPrefabs.Length)];
            GameObject seller = Instantiate(prefab, spawnPoints[i].position, Quaternion.Euler(0f, spawnPoints[i].eulerAngles.y, 0f));

            SellerNPC npc = seller.GetComponent<SellerNPC>();
            if (npc != null)
            {
                int randomStock = Random.Range(4, 6);

                Item selectedItem = (i < availableItems.Length)
                    ? availableItems[i]
                    : availableItems[Random.Range(0, availableItems.Length)];

                npc.InitializeSeller(selectedItem, randomStock);

                SellerTypeIdentifier typeId = seller.GetComponent<SellerTypeIdentifier>();
                Renderer renderer = seller.GetComponentInChildren<Renderer>();

                if (typeId != null && renderer != null)
                {
                    Texture randomTexture = GetRandomTextureForType(typeId.sellerType);
                    if (randomTexture != null)
                    {
                        renderer.material.SetTexture("_Main_Texture", randomTexture);
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
