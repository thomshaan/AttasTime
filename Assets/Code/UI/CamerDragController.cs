using UnityEngine;
using Cinemachine;

public class CameraDragController : MonoBehaviour
{
    [Header("References")]
    public CinemachineFreeLook freeLookCam;
    public Transform northReference;

    [Header("Settings")]
    public float dragSensitivity = 2f;
    public float recenterSpeed = 2f;

    private bool isDragging = false;
    private float targetHeading = 0f;
    private float currentHeading = 0f;
    private float lastDragTime = 0f;

    void Start()
    {
        if (freeLookCam != null)
            currentHeading = freeLookCam.m_XAxis.Value;
    }

    void Update()
    {
        HandleDragInput();
        UpdateCameraRotation();
    }

    void HandleDragInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            lastDragTime = Time.time;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            lastDragTime = Time.time;
        }

        if (isDragging && Input.GetMouseButton(0))
        {
            float mouseX = Input.GetAxis("Mouse X");
            freeLookCam.m_XAxis.Value += mouseX * dragSensitivity;
            currentHeading = freeLookCam.m_XAxis.Value;
        }
    }

    void UpdateCameraRotation()
    {
        if (!isDragging && Time.time - lastDragTime > 0.25f)
        {
            float northAngle = GetNorthAngle();
            currentHeading = Mathf.LerpAngle(currentHeading, northAngle, Time.deltaTime * recenterSpeed);
            freeLookCam.m_XAxis.Value = currentHeading;
        }
    }

    float GetNorthAngle()
    {
        if (northReference != null)
        {
            Vector3 dir = northReference.forward;
            float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
            return angle;
        }

        return 0f; // default to Z+
    }
}
