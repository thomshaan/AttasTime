using UnityEngine;

[ExecuteInEditMode]

public class TerrainTextureToWalkable : MonoBehaviour
{
    public Terrain terrain;
    public int[] textureIndices; // ← Now it's an array
    public float threshold = 0.5f;
    public GameObject walkablePrefab;
    public float spacing = 2f;

    void Start()
    {
        TerrainData terrainData = terrain.terrainData;
        float[,,] splatmapData = terrainData.GetAlphamaps(0, 0, terrainData.alphamapWidth, terrainData.alphamapHeight);

        for (int x = 0; x < terrainData.alphamapWidth; x += Mathf.RoundToInt(spacing))
        {
            for (int y = 0; y < terrainData.alphamapHeight; y += Mathf.RoundToInt(spacing))
            {
                bool isWalkable = false;

                foreach (int index in textureIndices)
                {
                    float texMix = splatmapData[y, x, index];
                    if (texMix > threshold)
                    {
                        isWalkable = true;
                        break;
                    }
                }

                if (isWalkable)
                {
                    Vector3 worldPos = TerrainToWorldPosition(x, y, terrainData, terrain);
                    Instantiate(walkablePrefab, worldPos, Quaternion.identity, transform);
                }
            }
        }
    }

    Vector3 TerrainToWorldPosition(int x, int y, TerrainData data, Terrain terrain)
    {
        float xPos = ((float)x / data.alphamapWidth) * data.size.x;
        float zPos = ((float)y / data.alphamapHeight) * data.size.z;
        float yPos = terrain.SampleHeight(new Vector3(xPos, 0, zPos));
        return terrain.transform.position + new Vector3(xPos, yPos, zPos);
    }
}
