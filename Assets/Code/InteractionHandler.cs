using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class InteractionHandler : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private float interactDistance = 3f;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private GameObject interactionUI;
    [SerializeField] private Button interactBtn;

    [Header("Cinemachine Cameras")]
    [SerializeField] private CinemachineVirtualCamera mainCam;
    [SerializeField] private CinemachineVirtualCamera interactionCam;

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
            currentInteractable = hit.collider.GetComponent<IInteractable>();

            if (currentInteractable != null)
            {
                interactionUI.SetActive(true);
                SwitchToInteractionCam();

                if (interactBtn != null && interactBtn.onClick.GetPersistentEventCount() == 0)
                {
                    interactBtn.onClick.AddListener(TriggerInteraction);
                }

                return;
            }
        }

        currentInteractable = null;
        interactionUI.SetActive(false);
        SwitchToMainCam();

        if (interactBtn != null)
            interactBtn.onClick.RemoveAllListeners();
    }

    public void TriggerInteraction()
    {
        if (currentInteractable != null)
        {
            currentInteractable.Interact();
        }
    }

    private void SwitchToInteractionCam()
    {
        if (mainCam != null)
        {
            mainCam.Priority = 10;
        }

        if (interactionCam != null)
        {
            interactionCam.gameObject.SetActive(true); // Enable it
            interactionCam.Priority = 20;
        }
    }

    private void SwitchToMainCam()
    {
        if (mainCam != null)
        {
            mainCam.Priority = 20;
        }

        if (interactionCam != null)
        {
            interactionCam.Priority = 10;
            interactionCam.gameObject.SetActive(false); // Disable it to save performance
        }
    }
}
