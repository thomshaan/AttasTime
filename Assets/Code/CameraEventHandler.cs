using UnityEngine;
using Cinemachine;

public class CameraEventHandler : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera interactionCam;

    public void OnCameraActivated(ICinemachineCamera newCam, ICinemachineCamera oldCam)
    {
        if (newCam == interactionCam)
        {
            Debug.Log("Interaction camera activated!");
            // 🔽 Add any logic here:
            // Show UI, pause game, play sfx, etc.
        }
    }
}
