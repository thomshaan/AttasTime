using UnityEngine;

public class InteractionHandler : MonoBehaviour
{
    [SerializeField] private float interactDistance = 3f;
    [SerializeField] private LayerMask interactableLayer;

    private float interactCooldown = 0.2f;
    private float lastInteractTime = -1f;
    private IInteractable currentInteractable;

    void Update()
    {
        CheckForInteractables();

        // Tekan tombol "Submit" (simulasi trigger / space di XR Device Simulator)
        if (currentInteractable != null && Input.GetButtonDown("Submit"))
        {
            TriggerInteraction();
        }
    }

    private void CheckForInteractables()
    {
        Ray ray = new Ray(transform.position, transform.forward); // Sesuaikan asal ray jika pakai gaze/controller
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance, interactableLayer))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                currentInteractable = interactable;
                return;
            }
        }

        currentInteractable = null;
    }

    private void TriggerInteraction()
    {
        if (Time.time - lastInteractTime < interactCooldown) return;

        if (currentInteractable != null)
        {
            lastInteractTime = Time.time;
            currentInteractable.Interact();
        }
    }
}
