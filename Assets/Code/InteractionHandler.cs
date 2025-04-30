using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InteractionHandler : MonoBehaviour
{
    [SerializeField] private float interactDistance = 3f;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private GameObject interactionUI;
    [SerializeField] private Button interactBtn; // Reference to your UI button (optional for click setup)

    private IInteractable currentInteractable;

    void Update()
    {
        CheckForInteractables();

        // Keyboard input (e.g. for desktop)
        if (currentInteractable != null && Input.GetButtonDown("Submit"))
        {
            currentInteractable.Interact();
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

                // Optional: If using button component, assign click event only once
                if (interactBtn != null && interactBtn.onClick.GetPersistentEventCount() == 0)
                {
                    interactBtn.onClick.AddListener(TriggerInteraction);
                }
            }
        }
        else
        {
            currentInteractable = null;
            interactionUI.SetActive(false);

            if (interactBtn != null)
                interactBtn.onClick.RemoveAllListeners();
        }
    }

    // ✅ This is what you need for the button's OnClick event
    public void TriggerInteraction()
    {
        if (currentInteractable != null)
        {
            currentInteractable.Interact();
        }
    }
}
