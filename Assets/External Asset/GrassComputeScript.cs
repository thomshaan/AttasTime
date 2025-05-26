// @Minionsart version
// credits  to  forkercat https://gist.github.com/junhaowww/fb6c030c17fe1e109a34f1c92571943f
// and  NedMakesGames https://gist.github.com/NedMakesGames/3e67fabe49e2e3363a657ef8a6a09838
// for the base setup for compute shaders
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class GrassComputeScript : MonoBehaviour
{
    public bool autoUpdate;
    private Camera m_MainCamera;
    public SO_GrassSettings currentPresets;
    ShaderInteractor[] interactors;
    [SerializeField, HideInInspector]
    List<GrassData> grassData = new List<GrassData>();
    List<int> grassVisibleIDList = new List<int>();
    private bool m_Initialized;
    private ComputeBuffer m_SourceVertBuffer;
    private ComputeBuffer m_DrawBuffer;
    private ComputeBuffer m_ArgsBuffer;
    private ComputeBuffer m_VisibleIDBuffer;
    [SerializeField] Material m_InstantiatedMaterial;
    private ComputeShader m_InstantiatedComputeShader;
    private int m_IdGrassKernel;
    private int m_DispatchSize;
    uint threadGroupSize;

    private const int SOURCE_VERT_STRIDE = sizeof(float) * (3 + 3 + 2 + 3);
    private const int DRAW_STRIDE = sizeof(float) * (3 + 3 + ((3 + 2) * 3));
    Bounds bounds;

    private uint[] argsBufferReset = new uint[5] { 0, 1, 0, 0, 0 };
    CullingTreeNode cullingTree;
    List<Bounds> BoundsListVis = new List<Bounds>();
    List<CullingTreeNode> leaves = new List<CullingTreeNode>();
    Plane[] cameraFrustumPlanes = new Plane[6];
    float cameraOriginalFarPlane;
    List<int> empty = new List<int>();
    Vector3 m_cachedCamPos;
    Quaternion m_cachedCamRot;
    bool m_fastMode;
    int shaderID;
    int maxBufferSize = 2500000;

    public List<GrassData> SetGrassPaintedDataList { get => grassData; set => grassData = value; }

#if UNITY_EDITOR
    SceneView view;

    void OnDestroy() => SceneView.duringSceneGui -= this.OnScene;

    void OnScene(SceneView scene)
    {
        view = scene;
        m_MainCamera = Application.isPlaying ? Camera.main : view?.camera;
    }

    private void OnValidate() => m_MainCamera = Application.isPlaying ? Camera.main : view?.camera;
#endif

    private void OnEnable()
    {
        if (m_Initialized) OnDisable();
        MainSetup(true);
    }

    void MainSetup(bool full)
    {
#if UNITY_EDITOR
        SceneView.duringSceneGui += this.OnScene;
        m_MainCamera = Application.isPlaying ? Camera.main : view?.camera;
#endif
        if (Application.isPlaying) m_MainCamera = Camera.main;

        if (grassData.Count == 0 || currentPresets.shaderToUse == null || currentPresets.materialToUse == null)
        {
            Debug.LogWarning("Missing Grass Data or Resources", this);
            return;
        }

        PopulateEmptyList(grassData.Count);
        m_Initialized = true;
        m_InstantiatedComputeShader = Instantiate(currentPresets.shaderToUse);
        m_InstantiatedMaterial = Instantiate(currentPresets.materialToUse);

        int numSourceVertices = grassData.Count;
        int maxBladesPerVertex = Mathf.Max(1, currentPresets.allowedBladesPerVertex);
        int maxSegmentsPerBlade = Mathf.Max(1, currentPresets.allowedSegmentsPerBlade);

        m_SourceVertBuffer = new ComputeBuffer(numSourceVertices, SOURCE_VERT_STRIDE, ComputeBufferType.Structured, ComputeBufferMode.Immutable);
        m_SourceVertBuffer.SetData(grassData);
        m_DrawBuffer = new ComputeBuffer(maxBufferSize, DRAW_STRIDE, ComputeBufferType.Append);
        m_ArgsBuffer = new ComputeBuffer(1, argsBufferReset.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        m_VisibleIDBuffer = new ComputeBuffer(grassData.Count, sizeof(int), ComputeBufferType.Structured);

        m_IdGrassKernel = m_InstantiatedComputeShader.FindKernel("Main");

        m_InstantiatedComputeShader.SetBuffer(m_IdGrassKernel, "_SourceVertices", m_SourceVertBuffer);
        m_InstantiatedComputeShader.SetBuffer(m_IdGrassKernel, "_DrawTriangles", m_DrawBuffer);
        m_InstantiatedComputeShader.SetBuffer(m_IdGrassKernel, "_IndirectArgsBuffer", m_ArgsBuffer);
        m_InstantiatedComputeShader.SetBuffer(m_IdGrassKernel, "_VisibleIDBuffer", m_VisibleIDBuffer);
        m_InstantiatedMaterial.SetBuffer("_DrawTriangles", m_DrawBuffer);
        m_InstantiatedComputeShader.SetInt("_NumSourceVertices", numSourceVertices);

        shaderID = Shader.PropertyToID("_PositionsMoving");
        m_InstantiatedComputeShader.GetKernelThreadGroupSizes(m_IdGrassKernel, out threadGroupSize, out _, out _);
        m_DispatchSize = Mathf.CeilToInt(grassData.Count / threadGroupSize);
        SetGrassDataBase(full);
        if (full) UpdateBounds();
        SetupQuadTree(full);
    }

    void UpdateBounds()
    {
        bounds = new Bounds(grassData[0].position, Vector3.one);
        for (int i = 0; i < grassData.Count; i++) bounds.Encapsulate(grassData[i].position);
    }

    void SetupQuadTree(bool full)
    {
        if (full)
        {
            cullingTree = new CullingTreeNode(bounds, currentPresets.cullingTreeDepth);
            cullingTree.RetrieveAllLeaves(leaves);
            for (int i = 0; i < grassData.Count; i++) cullingTree.FindLeaf(grassData[i].position, i);
            cullingTree.ClearEmpty();
        }
        else
        {
            GrassFastList(grassData.Count);
            m_VisibleIDBuffer.SetData(grassVisibleIDList);
        }
    }

    void GrassFastList(int count) => grassVisibleIDList = Enumerable.Range(0, count).ToList();

    void PopulateEmptyList(int count)
    {
        empty = new List<int>(count);
        empty.InsertRange(0, Enumerable.Repeat(-1, count));
    }

    void GetFrustumData()
    {
        if (m_MainCamera == null) return;
        if (m_cachedCamRot == m_MainCamera.transform.rotation && m_cachedCamPos == m_MainCamera.transform.position && Application.isPlaying) return;

        cameraOriginalFarPlane = m_MainCamera.farClipPlane;
        m_MainCamera.farClipPlane = currentPresets.maxDrawDistance;
        GeometryUtility.CalculateFrustumPlanes(m_MainCamera, cameraFrustumPlanes);
        m_MainCamera.farClipPlane = cameraOriginalFarPlane;

        if (!m_fastMode)
        {
            BoundsListVis.Clear();
            m_VisibleIDBuffer.SetData(empty);
            grassVisibleIDList.Clear();
            cullingTree.RetrieveLeaves(cameraFrustumPlanes, BoundsListVis, grassVisibleIDList);
            m_VisibleIDBuffer.SetData(grassVisibleIDList);
        }

        m_cachedCamPos = m_MainCamera.transform.position;
        m_cachedCamRot = m_MainCamera.transform.rotation;
    }

    private void OnDisable()
    {
        if (m_Initialized)
        {
            if (Application.isPlaying)
            {
                Destroy(m_InstantiatedComputeShader);
                Destroy(m_InstantiatedMaterial);
            }
            else
            {
                DestroyImmediate(m_InstantiatedComputeShader);
                DestroyImmediate(m_InstantiatedMaterial);
            }
            m_SourceVertBuffer?.Release();
            m_DrawBuffer?.Release();
            m_ArgsBuffer?.Release();
            m_VisibleIDBuffer?.Release();
        }
        m_Initialized = false;
    }

    private void Update()
    {
        if (!Application.isPlaying && autoUpdate && !m_fastMode)
        {
            OnDisable();
            OnEnable();
        }
        if (!m_Initialized) return;

        GetFrustumData();
        SetGrassDataUpdate();
        m_DrawBuffer.SetCounterValue(0);
        m_ArgsBuffer.SetData(argsBufferReset);
        m_DispatchSize = Mathf.CeilToInt(grassVisibleIDList.Count / threadGroupSize);
        if (grassVisibleIDList.Count > 0) m_DispatchSize += 1;

        if (m_DispatchSize > 0)
        {
            m_InstantiatedComputeShader.Dispatch(m_IdGrassKernel, m_DispatchSize, 1, 1);
            Graphics.DrawProceduralIndirect(m_InstantiatedMaterial, bounds, MeshTopology.Triangles,
            m_ArgsBuffer, 0, null, null, currentPresets.castShadow, true, gameObject.layer);
        }
    }

    private void SetGrassDataBase(bool full)
    {
        m_InstantiatedComputeShader.SetFloat("_Time", Time.time);
        m_InstantiatedComputeShader.SetFloat("_GrassRandomHeightMin", currentPresets.grassRandomHeightMin);
        m_InstantiatedComputeShader.SetFloat("_GrassRandomHeightMax", currentPresets.grassRandomHeightMax);
        m_InstantiatedComputeShader.SetFloat("_WindSpeed", currentPresets.windSpeed);
        m_InstantiatedComputeShader.SetFloat("_WindStrength", currentPresets.windStrength);

        if (full)
        {
            m_InstantiatedComputeShader.SetFloat("_MinFadeDist", currentPresets.minFadeDistance);
            m_InstantiatedComputeShader.SetFloat("_MaxFadeDist", currentPresets.maxDrawDistance);
            interactors = FindObjectsOfType<ShaderInteractor>();
        }
        else
        {
            if (grassData.Count > 200000)
            {
                m_InstantiatedComputeShader.SetFloat("_MinFadeDist", 40f);
                m_InstantiatedComputeShader.SetFloat("_MaxFadeDist", 50f);
            }
            else
            {
                m_InstantiatedComputeShader.SetFloat("_MinFadeDist", currentPresets.minFadeDistance);
                m_InstantiatedComputeShader.SetFloat("_MaxFadeDist", currentPresets.maxDrawDistance);
            }
        }

        m_InstantiatedComputeShader.SetFloat("_InteractorStrength", currentPresets.affectStrength);
        m_InstantiatedComputeShader.SetFloat("_BladeRadius", currentPresets.bladeRadius);
        m_InstantiatedComputeShader.SetFloat("_BladeForward", currentPresets.bladeForwardAmount);
        m_InstantiatedComputeShader.SetFloat("_BladeCurve", Mathf.Max(0, currentPresets.bladeCurveAmount));
        m_InstantiatedComputeShader.SetFloat("_BottomWidth", currentPresets.bottomWidth);
        m_InstantiatedComputeShader.SetInt("_MaxBladesPerVertex", currentPresets.allowedBladesPerVertex);
        m_InstantiatedComputeShader.SetInt("_MaxSegmentsPerBlade", currentPresets.allowedSegmentsPerBlade);
        m_InstantiatedComputeShader.SetFloat("_MinHeight", currentPresets.MinHeight);
        m_InstantiatedComputeShader.SetFloat("_MinWidth", currentPresets.MinWidth);
        m_InstantiatedComputeShader.SetFloat("_MaxHeight", currentPresets.MaxHeight);
        m_InstantiatedComputeShader.SetFloat("_MaxWidth", currentPresets.MaxWidth);
        m_InstantiatedMaterial.SetColor("_TopTint", currentPresets.topTint);
        m_InstantiatedMaterial.SetColor("_BottomTint", currentPresets.bottomTint);
    }

    public void Reset() { m_fastMode = false; OnDisable(); MainSetup(true); }
    public void ResetFaster() { m_fastMode = true; OnDisable(); MainSetup(false); }

    private void SetGrassDataUpdate()
    {
        m_InstantiatedComputeShader.SetFloat("_Time", Time.time);
        m_InstantiatedComputeShader.SetMatrix("_LocalToWorld", transform.localToWorldMatrix);
        if (interactors != null && interactors.Length > 0)
        {
            Vector4[] positions = new Vector4[interactors.Length];
            for (int i = 0; i < interactors.Length; i++)
            {
                if (interactors[i] == null) continue;
                positions[i] = new Vector4(
                    interactors[i].transform.position.x,
                    interactors[i].transform.position.y,
                    interactors[i].transform.position.z,
                    interactors[i].radius
                );
            }
            m_InstantiatedComputeShader.SetVectorArray(shaderID, positions);
            m_InstantiatedComputeShader.SetFloat("_InteractorsLength", interactors.Length);
        }
        if (m_MainCamera != null)
        {
            m_InstantiatedComputeShader.SetVector("_CameraPositionWS", m_MainCamera.transform.position);
        }
#if UNITY_EDITOR
        else if (view != null && view.camera != null)
        {
            m_InstantiatedComputeShader.SetVector("_CameraPositionWS", view.camera.transform.position);
        }
#endif
    }

    void OnDrawGizmos()
    {
        if (currentPresets && currentPresets.drawBounds)
        {
            Gizmos.color = new Color(0, 1, 0, 0.3f);
            foreach (var bound in BoundsListVis) Gizmos.DrawWireCube(bound.center, bound.size);
            Gizmos.color = new Color(1, 0, 0, 0.3f);
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }
    }
}

[System.Serializable]
[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
public struct GrassData
{
    public Vector3 position;
    public Vector3 normal;
    public Vector2 length;
    public Vector3 color;
}
