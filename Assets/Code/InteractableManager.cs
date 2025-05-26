using UnityEngine;
using UnityEngine.UI;

public class InteractManager : MonoBehaviour
{
    public float interactRadius = 2.5f;
    public LayerMask interactableLayer;
    public KeyCode interactKey = KeyCode.E;
    public GameObject interactionUIPrompt;
    public Text promptText;

    private IInteractable currentTarget;

    void Update()
    {
        ScanForInteractables();

        if (currentTarget != null && Input.GetKeyDown(interactKey))
        {
            currentTarget.Interact();
        }
    }

    void ScanForInteractables()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, interactRadius, interactableLayer);
        float closestDist = Mathf.Infinity;
        IInteractable nearest = null;

        foreach (var hit in hits)
        {
            IInteractable candidate = hit.GetComponent<IInteractable>();
            if (candidate != null)
            {
                float dist = Vector3.Distance(transform.position, hit.transform.position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    nearest = candidate;
                }
            }
        }

        if (nearest != null)
        {
            currentTarget = nearest;
            ShowPrompt(currentTarget.GetInteractionPrompt());
        }
        else
        {
            currentTarget = null;
            HidePrompt();
        }
    }

    void ShowPrompt(string message)
    {
        if (interactionUIPrompt != null && promptText != null)
        {
            interactionUIPrompt.SetActive(true);
            promptText.text = message;
        }
    }

    void HidePrompt()
    {
        if (interactionUIPrompt != null)
        {
            interactionUIPrompt.SetActive(false);
        }
    }
}
