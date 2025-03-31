using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshSurface))]
public class NavMeshGenA : MonoBehaviour
{
    private Terrain terrain;
    private TerrainData terrainData;
    private Vector3 terrainPos;

    public string NavAgentLayer = "Default";
    public string defaultarea = "Walkable";
    public bool includeTrees;
    public float timeLimitInSecs = 20f;
    public int step = 1;
    public List<string> areaID;
    
    [SerializeField] private bool _destroyTempObjects;
    [SerializeField] private bool _break;

    [ContextMenu("Generate NavAreas")]
    void Build()
    {
        StartCoroutine(GenMeshes());
    }
    
    IEnumerator GenMeshes()
    {
        terrain = GetComponent<Terrain>();
        terrainData = terrain.terrainData;
        terrainPos = terrain.transform.position;

        Vector3 size = terrain.terrainData.size;
        Vector3 tpos = terrain.GetPosition();
        float minX = tpos.x;
        float maxX = minX + size.x;
        float minZ = tpos.z;
        float maxZ = minZ + size.z;

        GameObject attachParent = new GameObject("Delete me");
        attachParent.transform.SetParent(terrain.transform);
        attachParent.transform.localPosition = Vector3.zero;

        yield return null;

        int terrainLayer = LayerMask.NameToLayer(NavAgentLayer);
        int defaultWalkableArea = NavMesh.GetAreaFromName(defaultarea);

        float[,,] splatmapData = terrainData.GetAlphamaps(0, 0, terrainData.alphamapWidth, terrainData.alphamapHeight);

        int alphaWidth = splatmapData.GetLength(0);
        int alphaHeight = splatmapData.GetLength(1);
        float startTime = Time.realtimeSinceStartup;
        float xStepSize = size.x / alphaWidth;
        float zStepSize = size.z / alphaHeight;

        for (int dx = 0; dx < alphaWidth; dx += step)
        {
            yield return null;
            float xOff = size.x * (dx / (float)alphaWidth);
            for (int dz = 0; dz < alphaHeight; dz += step)
            {
                if (_break) yield break;
                if (dx >= alphaWidth || dz >= alphaHeight) continue;

                float zOff = size.z * (dz / (float)alphaHeight);
                int surface = GetMainTextureA(dx, dz, ref splatmapData);
                if (!areaID.Contains(surface.ToString())) continue;

                if (Time.realtimeSinceStartup > startTime + timeLimitInSecs)
                {
                    Debug.Log("Time limit exceeded");
                    goto escape;
                }

                Vector3 pos = new Vector3(minX + xOff, 0, minZ + zOff);
                float height = terrain.SampleHeight(pos);
                
                GameObject obj = new GameObject();
                obj.transform.SetParent(attachParent.transform);
                obj.transform.position = new Vector3(pos.x, height, pos.z);
                
                NavMeshModifierVolume nmmv = obj.AddComponent<NavMeshModifierVolume>();
                nmmv.size = new Vector3(xStepSize * step, 1, zStepSize * step);
                nmmv.center = Vector3.zero;
                nmmv.area = 0;
            }
        }
    escape:
        
        if (includeTrees)
        {
            Debug.Log("Processing trees");
            foreach (TreeInstance inst in terrainData.treeInstances)
            {
                Vector3 pos = Vector3.Scale(inst.position, terrainData.size);
                GameObject tree = Instantiate(terrainData.treePrototypes[inst.prototypeIndex].prefab);
                tree.transform.SetParent(attachParent.transform);
                tree.transform.position = new Vector3(pos.x, pos.y, pos.z);
                tree.isStatic = true;
            }
        }
        
        foreach (NavMeshSurface nsurface in GetComponents<NavMeshSurface>())
        {
            nsurface.BuildNavMesh();
            yield return null;
        }

        if (_destroyTempObjects)
            DestroyImmediate(attachParent.gameObject);
    }

    private float[] GetTextureMixA(int x, int z, ref float[,,] splatmapData)
    {
        x = Mathf.Clamp(x, 0, splatmapData.GetLength(0) - 1);
        z = Mathf.Clamp(z, 0, splatmapData.GetLength(1) - 1);

        int layers = splatmapData.GetLength(2);
        float[] cellMix = new float[layers];
        
        try
        {
            for (int n = 0; n < layers; n++)
            {
                cellMix[n] = splatmapData[x, z, n];
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error accessing splatmapData[{x}, {z}, ...]: {e.Message}");
        }
        return cellMix;
    }

    private int GetMainTextureA(int x, int z, ref float[,,] splatmapData)
    {
        x = Mathf.Clamp(x, 0, splatmapData.GetLength(0) - 1);
        z = Mathf.Clamp(z, 0, splatmapData.GetLength(1) - 1);

        float[] mix = GetTextureMixA(x, z, ref splatmapData);
        float maxMix = 0;
        int maxIndex = 0;

        for (int n = 0; n < mix.Length; n++)
        {
            if (mix[n] > maxMix)
            {
                maxIndex = n;
                maxMix = mix[n];
            }
        }
        return maxIndex;
    }
}