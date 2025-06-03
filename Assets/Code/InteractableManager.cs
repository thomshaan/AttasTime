using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class InteractManager : MonoBehaviour
{
    public float interactRadius = 2.5f;
    public LayerMask interactableLayer;
    public KeyCode interactKey = KeyCode.E;

    public GameObject interactionUIPrompt;
    public Text promptText;
    public Button interactButton;
    public Text interactButtonText;

    public CinemachineFreeLook mainCam;
    public CinemachineVirtualCamera interactionCam;

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
            if (currentTarget != nearest)
            {
                currentTarget = nearest;
                ShowPrompt(currentTarget.GetInteractionPrompt());
                SwitchToInteractionCam();
            }
        }
        else
        {
            currentTarget = null;
            HidePrompt();
            SwitchToMainCam();
        }
    }

    void ShowPrompt(string message)
    {
        if (interactButton != null && interactButtonText != null)
        {
            interactButton.gameObject.SetActive(true);
            interactButtonText.text = message;

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

    void SwitchToInteractionCam()
    {
        if (mainCam != null) mainCam.Priority = 10;
        if (interactionCam != null)
        {
            interactionCam.gameObject.SetActive(true);
            interactionCam.Priority = 20;
        }
    }

    void SwitchToMainCam()
    {
        if (mainCam != null) mainCam.Priority = 20;
        if (interactionCam != null)
        {
            interactionCam.Priority = 10;
            interactionCam.gameObject.SetActive(false);
        }
    }
}

