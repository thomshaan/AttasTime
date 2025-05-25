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
            if (!string.IsNullOrEmpty(targetSceneName))
            {
                Debug.Log($"Player entered trigger, loading scene: {targetSceneName} with spawn ID: {targetSpawnID}");

                // Simpan spawn ID
                SaveManager.spawnTargetID = targetSpawnID;

                // Hancurkan player sebelum load scene
                Destroy(other.gameObject); // penting agar tidak ikut ke DontDestroyOnLoad

                // Load scene baru
                SceneManager.LoadScene(targetSceneName);
            }
            else
            {
                Debug.LogWarning("Target scene name belum di-set di " + gameObject.name);
            }
        }
    }

}
