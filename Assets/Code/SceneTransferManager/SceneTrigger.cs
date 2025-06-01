using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTrigger : MonoBehaviour
{
    [Tooltip("Nama scene yang akan diload saat player masuk trigger")]
    public string targetSceneName;

    [Tooltip("Spawn ID yang akan digunakan di scene tujuan")]
    public string targetSpawnID;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"[SceneTrigger] Player entered trigger, loading scene: {targetSceneName} with spawn ID: {targetSpawnID}");

            // Set spawn target dan flag perpindahan via trigger
            SaveManager.spawnTargetID = targetSpawnID;
            SaveManager.isSceneTriggerSpawn = true;

            // Hancurkan player agar SaveManager spawn ulang di scene baru
            Destroy(other.gameObject);

            // Load scene tujuan
            SaveManager.Instance.SaveBeforeSceneChange(targetSceneName);
        }
    }
}
