using UnityEngine;
using UnityEngine.SceneManagement;

public class DestroyOnMainMenu : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Mainmenu") // Huruf kecil 'm' di 'menu', sesuai Build Settings
        {
            Destroy(gameObject);
            Debug.Log($"Destroyed {gameObject.name} on scene {scene.name}");
        }
    }
}
