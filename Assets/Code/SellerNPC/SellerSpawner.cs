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
    private List<GameObject> spawnedSellers = new List<GameObject>();

    void Update()
    {
        float time = LightingManager.Instance.TimeOfDay;

        if (time >= 7f && time < 17f && spawnedSellers.Count == 0)
        {
            SpawnSellers();
        }
        else if ((time < 7f || time >= 17f) && spawnedSellers.Count > 0)
        {
            DespawnSellers();
        }
    }

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
            spawnedSellers.Add(seller);

            SellerNPC npc = seller.GetComponent<SellerNPC>();
            if (npc != null)
            {
                int stock = Random.Range(4, 6);
                Item selectedItem = (i < availableItems.Length) ? availableItems[i] : availableItems[Random.Range(0, availableItems.Length)];
                npc.InitializeSeller(selectedItem, stock);

                var typeId = seller.GetComponent<SellerTypeIdentifier>();
                var renderer = seller.GetComponentInChildren<Renderer>();
                if (typeId != null && renderer != null)
                {
                    Texture texture = GetRandomTextureForType(typeId.sellerType);
                    if (texture != null)
                        renderer.material.SetTexture("_Main_Texture", texture);
                }
            }
        }
    }

    void DespawnSellers()
    {
        foreach (var seller in spawnedSellers)
        {
            Destroy(seller);
        }
        spawnedSellers.Clear();
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
