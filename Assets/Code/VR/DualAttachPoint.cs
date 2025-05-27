using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DualAttachPoint : MonoBehaviour
{
    public Transform leftAttachPoint;    // Assign attach point untuk tangan kiri
    public Transform rightAttachPoint;   // Assign attach point untuk tangan kanan

    private XRGrabInteractable grabInteractable;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();

        // Subscribe ke event selectEntered dan selectExited
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
    }

    private void OnDestroy()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrab);
        grabInteractable.selectExited.RemoveListener(OnRelease);
    }

    // Event handler untuk grab (selectEntered)
    private void OnGrab(SelectEnterEventArgs args)
    {
        XRBaseInteractor interactor = args.interactorObject as XRBaseInteractor;
        if (interactor != null)
        {
            if (interactor.name.Contains("Left"))
            {
                grabInteractable.attachTransform = leftAttachPoint;
            }
            else if (interactor.name.Contains("Right"))
            {
                grabInteractable.attachTransform = rightAttachPoint;
            }
        }
    }

    // Event handler untuk release (selectExited)
    private void OnRelease(SelectExitEventArgs args)
    {
        grabInteractable.attachTransform = null;
    }
}
