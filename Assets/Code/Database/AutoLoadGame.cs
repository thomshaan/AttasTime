using UnityEngine;
using System.Collections;

public class AutoLoadGame : MonoBehaviour
{
    public SaveManager saveManager;
    public int slotToLoad = 1;

    IEnumerator Start()
    {
        yield return new WaitUntil(() => saveManager.player != null);
        yield return new WaitForSeconds(0.1f); // just to ensure everything is ready

        saveManager.LoadGame();
    }
}
