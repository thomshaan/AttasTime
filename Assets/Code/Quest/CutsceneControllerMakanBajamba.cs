using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

public class CutsceneControllerMakanBajamba : MonoBehaviour
{
    [Header("Camera & Fade")]
    public CinemachineVirtualCamera cutsceneCamera;
    public UIOnOffManager uiFader;

    [Header("Spawn Points & Prefabs")]
    public Transform attaSpawnPoint;
    public GameObject attaPrefab;
    public Transform syekhSpawnPoint;
    public GameObject syekhPrefab;
    public Transform[] extraNpcsSpawn;
    public GameObject[] extraNpcsPrefabs;

    [Header("Cutscene Data")]
    public DialogData doaDialog;
    public DialogData makanDialog;
    public DialogData syekhClosingDialog;
    public float doaAnimDuration = 2f;
    public float makanAnimDuration = 2f;

    private GameObject attaInstance;
    private GameObject syekhInstance;
    private List<GameObject> extraInstances = new List<GameObject>();

    private List<BaseCharacterAnimatorHandler> animHandlers = new List<BaseCharacterAnimatorHandler>();

    public string fadePanelName = "FadePanel";

    public void StartCutscene()
    {
        StartCoroutine(CutsceneRoutine());
    }

    private IEnumerator CutsceneRoutine()
    {
        uiFader.Show(fadePanelName);
        yield return new WaitForSeconds(1f);

        cutsceneCamera.Priority = 20;
        SpawnCharacters();

        yield return PlayDoubleBoolAnimationAndDialog("duduk", "doa", doaAnimDuration, doaDialog);
        yield return PlayAnimationAndDialog("makan", makanAnimDuration, makanDialog);

        // Syekh says closing line
        DialogManager.Instance.StartDialog(syekhClosingDialog);
        yield return new WaitUntil(() => !DialogManager.Instance.dialogBox.activeSelf);

        cutsceneCamera.Priority = 0;
        uiFader.Hide(fadePanelName);
        yield return new WaitForSeconds(0.5f);

        // Quest is not yet completed here; player must report to Ibu
        QuestManager.Instance.UpdateQuestState(QuestState.ReadyToComplete);

        SaveManager.Instance.SaveGame();
    }

    private IEnumerator PlayDoubleBoolAnimationAndDialog(string preconditionAlias, string mainAlias, float duration, DialogData dialog)
    {
        foreach (var handler in animHandlers)
        {
            if (handler != null && handler.gameObject != null)
            {
                handler.SetBool(preconditionAlias, true);
                handler.SetBool(mainAlias, true);
            }
        }

        yield return new WaitForSeconds(duration);

        foreach (var handler in animHandlers)
        {
            if (handler != null && handler.gameObject != null)
            {
                handler.SetBool(preconditionAlias, false);
                handler.SetBool(mainAlias, false);
            }
        }

        DialogManager.Instance.StartDialog(dialog);
        yield return new WaitUntil(() => !DialogManager.Instance.dialogBox.activeSelf);
    }

    private IEnumerator PlayAnimationAndDialog(string animAlias, float duration, DialogData dialog)
    {
        foreach (var handler in animHandlers)
        {
            if (handler != null && handler.gameObject != null)
            {
                handler.PlayAnim(animAlias, duration);
            }
        }

        yield return new WaitForSeconds(duration);
        DialogManager.Instance.StartDialog(dialog);
        yield return new WaitUntil(() => !DialogManager.Instance.dialogBox.activeSelf);
    }

    private void SpawnCharacters()
    {
        animHandlers.Clear();

        attaInstance = Instantiate(attaPrefab, attaSpawnPoint.position, attaSpawnPoint.rotation);
        AddAnimHandler(attaInstance);

        syekhInstance = Instantiate(syekhPrefab, syekhSpawnPoint.position, syekhSpawnPoint.rotation);
        AddAnimHandler(syekhInstance);

        extraInstances.Clear();
        int spawnIndex = 0;
        while (spawnIndex < extraNpcsSpawn.Length)
        {
            for (int i = 0; i < extraNpcsPrefabs.Length && spawnIndex < extraNpcsSpawn.Length; i++)
            {
                var inst = Instantiate(extraNpcsPrefabs[i], extraNpcsSpawn[spawnIndex].position, extraNpcsSpawn[spawnIndex].rotation);
                extraInstances.Add(inst);
                AddAnimHandler(inst);
                spawnIndex++;
            }
        }
    }

    private void AddAnimHandler(GameObject obj)
    {
        var handler = obj.GetComponent<BaseCharacterAnimatorHandler>();
        if (handler != null)
        {
            animHandlers.Add(handler);
        }
    }
}
