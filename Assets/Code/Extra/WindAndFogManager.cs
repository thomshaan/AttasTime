using UnityEngine;

public class WindAndFogController : MonoBehaviour
{
    [Header("Wind Particle Settings")]
    public GameObject windParticlePrefab;
    public float spawnInterval = 5f;
    public Vector3 areaSize = new Vector3(50, 0, 50);
    public float particleLifetime = 8f;

    [Header("Fog Settings")]
    public GameObject groundFogObject;
    public string depthFadeProperty = "_DepthFade";
    public float fogFadeSpeed = 1.5f;
    public float fogVisibleValue = 30f;
    public float fogHiddenValue = 200f;

    private float timer = 0f;
    private Material fogMaterial;
    private float currentDepthFade;

    void Start()
    {
        if (groundFogObject != null)
        {
            Renderer fogRenderer = groundFogObject.GetComponent<Renderer>();
            if (fogRenderer != null)
            {
                fogMaterial = fogRenderer.material;
                currentDepthFade = fogMaterial.GetFloat(depthFadeProperty);
            }
        }
    }

    void Update()
    {
        float timeOfDay = LightingManager.Instance.TimeOfDay;

        HandleWindSpawning();
        HandleFogDepthFade(timeOfDay);
    }

    void HandleWindSpawning()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;
            Vector3 spawnPos = transform.position + new Vector3(
                Random.Range(-areaSize.x / 2f, areaSize.x / 2f),
                Random.Range(-areaSize.y / 2f, areaSize.y / 2f),
                Random.Range(-areaSize.z / 2f, areaSize.z / 2f)
            );

            Quaternion spawnRot = Quaternion.identity; // fixed rotation
            GameObject particle = Instantiate(windParticlePrefab, spawnPos, spawnRot);
            Destroy(particle, particleLifetime);
        }
    }

    void HandleFogDepthFade(float timeOfDay)
    {
        if (fogMaterial == null) return;

        bool shouldShowFog = (timeOfDay >= 19f || timeOfDay < 6f);
        float targetFade = shouldShowFog ? fogVisibleValue : fogHiddenValue;

        currentDepthFade = Mathf.MoveTowards(currentDepthFade, targetFade, Time.deltaTime * fogFadeSpeed);
        fogMaterial.SetFloat(depthFadeProperty, currentDepthFade);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, areaSize);
    }
}
