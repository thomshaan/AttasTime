using UnityEngine;
using UnityEditor;

public class WalkablePainter : MonoBehaviour
{
    [ContextMenu("Paint Walkable Areas")]
    void PaintWalkables()
    {
        Terrain terrain = GetComponent<Terrain>();
        if (terrain == null) return;

        TerrainData terrainData = terrain.terrainData;
        float[,,] splatmapData = terrainData.GetAlphamaps(0, 0, terrainData.alphamapWidth, terrainData.alphamapHeight);

        GameObject prefab = Resources.Load<GameObject>("WalkablePrefab"); // Place your prefab in Resources folder
        if (prefab == null) {
            Debug.LogError("WalkablePrefab not found in Resources folder!");
            return;
        }

        int[] indices = new int[] { 1, 3 }; // Your path texture indices
        float threshold = 0.5f;
        float spacing = 2f;

        for (int x = 0; x < terrainData.alphamapWidth; x += Mathf.RoundToInt(spacing))
        {
            for (int y = 0; y < terrainData.alphamapHeight; y += Mathf.RoundToInt(spacing))
            {
                bool shouldPlace = false;
                foreach (int index in indices)
                {
                    if (splatmapData[y, x, index] > threshold)
                    {
                        shouldPlace = true;
                        break;
                    }
                }

                if (shouldPlace)
                {
                    Vector3 worldPos = TerrainToWorldPosition(x, y, terrainData, terrain);
                    GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                    obj.transform.position = worldPos;
                    obj.transform.SetParent(this.transform); // Optional
                    obj.isStatic = true;
                }
            }
        }

        Debug.Log("Finished painting walkable prefabs.");
    }

    Vector3 TerrainToWorldPosition(int x, int y, TerrainData data, Terrain terrain)
    {
        float xPos = ((float)x / data.alphamapWidth) * data.size.x;
        float zPos = ((float)y / data.alphamapHeight) * data.size.z;
        float yPos = terrain.SampleHeight(new Vector3(xPos, 0, zPos));
        return terrain.transform.position + new Vector3(xPos, yPos, zPos);
    }
}
