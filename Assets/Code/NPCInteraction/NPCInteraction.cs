using UnityEngine;

public class NPCInteraction : MonoBehaviour, IInteractable
{
    [Header("NPC Info")]
    public string npcName;

    private INPCBehavior[] behaviors;

    private void Awake()
    {
        behaviors = GetComponents<INPCBehavior>();
    }

    public void Interact()
    {
        Debug.Log($"▶️ Interacting with {npcName}");

        foreach (var behavior in behaviors)
        {
            behavior.OnInteract();
        }
    }

    public string GetInteractionPrompt()
    {
        return $"Talk to {npcName}";
    }
}
