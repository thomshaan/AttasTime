using UnityEngine;

public class NPCDialogTrigger : MonoBehaviour, IInteractable
{
    [Header("Dialog Data ScriptableObject")]
    public DialogData dialogData;

    public void Interact()
    {
        if (dialogData != null)
        {
            DialogManager.Instance.StartDialog(dialogData);
        }
        else
        {
            Debug.LogWarning("[NPCDialogTrigger] DialogData belum diisi di " + gameObject.name);
        }
    }

    public string GetInteractionPrompt()
    {
        return "Talk";
    }
}
