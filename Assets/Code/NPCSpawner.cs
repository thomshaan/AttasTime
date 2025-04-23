using UnityEngine;
using UnityEngine.AI;


public class NPCSpawner : MonoBehaviour
{
    [Header("NPC Prefabs")]
    public GameObject[] npcPrefabs; // Assign different NPC prefabs if needed

    [Header("Spawn Settings")]
    public int totalNPCs = 15;
    public int maxNPCsPerHouse = 4;

    [Header("Waypoints")]
    public Transform market;
    public Transform cityCenter;
    public Transform mosque;
    public Transform[] houses;

    private void Start()
    {
        SpawnNPCs();
    }

    void SpawnNPCs()
    {
        if (npcPrefabs.Length == 0 || houses.Length == 0)
        {
            Debug.LogError("Please assign NPC prefabs and house Transforms in the inspector.");
            return;
        }

        int[] houseNPCCount = new int[houses.Length];

        for (int i = 0; i < totalNPCs; i++)
        {
            // Get available house index (with max cap)
            int houseIndex = GetAvailableHouseIndex(houseNPCCount);
            if (houseIndex == -1)
            {
                Debug.LogWarning("All houses reached maximum NPC capacity.");
                break;
            }

            houseNPCCount[houseIndex]++;
            Vector3 spawnPos = GetRandomSpawnPositionAround(houses[houseIndex].position, 2f);

            GameObject npcPrefab = npcPrefabs[Random.Range(0, npcPrefabs.Length)];
            GameObject npc = Instantiate(npcPrefab, spawnPos, Quaternion.identity);

            NPCSchedule schedule = npc.GetComponent<NPCSchedule>();
            if (schedule != null)
            {
                schedule.Market = market;
                schedule.CityCenter = cityCenter;
                schedule.Mosque = mosque;
                schedule.Houses = houses;
                schedule.AssignedHouseIndex = houseIndex;
                npc.name = $"NPC_{i}_House{houseIndex}";

                // Randomize their morning behavior
                schedule.morningDestination = (MorningDestination)Random.Range(0, 3); // 0: Market, 1: CityCenter, 2: Home
            }
            else
            {
                Debug.LogError("NPC prefab missing NPCSchedule script!");
            }
        }
    }

    int GetAvailableHouseIndex(int[] houseCounts)
    {
        // Try random first
        for (int attempt = 0; attempt < 20; attempt++)
        {
            int index = Random.Range(0, houses.Length);
            if (houseCounts[index] < maxNPCsPerHouse)
            {
                return index;
            }
        }

        // Fallback to first available
        for (int i = 0; i < houseCounts.Length; i++)
        {
            if (houseCounts[i] < maxNPCsPerHouse)
            {
                return i;
            }
        }

        return -1;
    }

    Vector3 GetRandomSpawnPositionAround(Vector3 center, float radius)
    {
        for (int i = 0; i < 10; i++)
        {
            Vector2 randomOffset = Random.insideUnitCircle * radius;
            Vector3 position = new Vector3(center.x + randomOffset.x, center.y, center.z + randomOffset.y);

            if (NavMesh.SamplePosition(position, out NavMeshHit hit, 2.0f, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }

        return center; // fallback if NavMesh not found
    }
}
