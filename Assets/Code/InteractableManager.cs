using UnityEngine;
using UnityEngine.UI;

public class InteractManager : MonoBehaviour
{
    public float interactRadius = 2.5f;
    public LayerMask interactableLayer;
    public KeyCode interactKey = KeyCode.E;
    public GameObject interactionUIPrompt;
    public Text promptText;
    public Button interactButton;      // InteractBtn dari Canvas
    public Text interactButtonText;    // Text di dalam button

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
        if (interactButton != null && interactButtonText != null)
        {
            interactButton.gameObject.SetActive(true);
            interactButtonText.text = message;

            // Hapus listener lama agar tidak dobel
            interactButton.onClick.RemoveAllListeners();

            if (currentTarget != null)
                interactButton.onClick.AddListener(() => currentTarget.Interact());
        }
    }

    void HidePrompt()
    {
        if (interactButton != null)
        {
            interactButton.gameObject.SetActive(false);
            interactButton.onClick.RemoveAllListeners();
        }
    }
}
