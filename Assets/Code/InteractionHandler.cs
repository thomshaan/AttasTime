using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class InteractionHandler : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private float interactDistance = 3f;
    [SerializeField] private LayerMask interactableLayer;

    [Header("UI")]
    [SerializeField] private GameObject interactionUI;
    [SerializeField] private Button interactButton;

    [Header("Cinemachine Cameras")]
    [SerializeField] private CinemachineFreeLook mainCam;
    [SerializeField] private CinemachineVirtualCamera interactionCam;

    private float interactCooldown = 0.2f;
    private float lastInteractTime = -1f;
    private IInteractable currentInteractable;

    void Update()
    {
        CheckForInteractables();

        if (currentInteractable != null && Input.GetButtonDown("Submit"))
        {
            TriggerInteraction();
        }
    }

    private void CheckForInteractables()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance, interactableLayer))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                if (currentInteractable != interactable)
                {
                    currentInteractable = interactable;

                    ShowInteractionUI(true);

                    interactButton.onClick.RemoveAllListeners();
                    interactButton.onClick.AddListener(TriggerInteraction);


                    SwitchToInteractionCam();
                }
                return;
            }
        }

        // No interactable
        currentInteractable = null;
        ShowInteractionUI(false);
        interactButton.onClick.RemoveAllListeners();
        SwitchToMainCam();
    }

    public void TriggerInteraction()
    {
        if (Time.time - lastInteractTime < interactCooldown) return; // skip if too soon

        if (currentInteractable != null)
        {
            lastInteractTime = Time.time;
            currentInteractable.Interact();
        }
    }

    private void ShowInteractionUI(bool show)
    {
        if (interactionUI != null)
            interactionUI.SetActive(show);
        if (interactButton != null)
            interactButton.gameObject.SetActive(show);
    }

    private void SwitchToInteractionCam()
    {
        if (mainCam != null) mainCam.Priority = 10;
        if (interactionCam != null)
        {
            interactionCam.gameObject.SetActive(true);
            interactionCam.Priority = 20;
        }
    }

    private void SwitchToMainCam()
    {
        if (mainCam != null) mainCam.Priority = 20;
        if (interactionCam != null)
        {
            interactionCam.Priority = 10;
            interactionCam.gameObject.SetActive(false);
        }
    }
}
