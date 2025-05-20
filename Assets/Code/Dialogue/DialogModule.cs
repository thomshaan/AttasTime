using UnityEngine;

public class DialogModule : MonoBehaviour, INPCBehavior
{
    [Header("Dialog Settings")]
    public DialogLine[] lines;

    private int index = 0;
    private bool inDialog = false;

    private Animator animator;
    private NPCMover mover;

    void Start()
    {
        animator = GetComponent<Animator>();
        mover = GetComponent<NPCMover>(); // Optional movement
    }

    public void OnInteract()
    {
        if (!inDialog)
        {
            index = 0;
            inDialog = true;
        }

        if (index < lines.Length)
        {
            var line = lines[index];

            Debug.Log($"🗨️ {line.text}");

            if (!string.IsNullOrEmpty(line.animationTrigger) && animator != null)
            {
                animator.SetTrigger(line.animationTrigger);
            }

            index++;
        }
        else
        {
            Debug.Log("▶️ Dialog finished.");
            inDialog = false;

            var finalTarget = lines[lines.Length - 1].moveTarget;
            if (finalTarget != null && mover != null)
            {
                mover.MoveTo(finalTarget.position);
            }
        }
    }
}
