using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ActivateOnInteract : MonoBehaviour
{
    public XRBaseInteractable interactable; // Reference to XR Simple Interactable component
    public GameObject objectToActivate;     // The GameObject you want to activate

    private void OnEnable()
    {
        interactable.selectEntered.AddListener(OnInteract);
    }

    private void OnDisable()
    {
        interactable.selectEntered.RemoveListener(OnInteract);
    }

    private void OnInteract(SelectEnterEventArgs args)
    {
        if (objectToActivate != null)
        {
            objectToActivate.SetActive(true);
        }
    }
}
