using UnityEngine;

public class FollowXROrigin : MonoBehaviour
{
    public Transform xrOrigin;
    public float rotationSmoothSpeed = 5f;

    void LateUpdate()
    {
        if (xrOrigin == null) return;

        // Ikuti posisi langsung
        transform.position = xrOrigin.position;

        // Ambil rotasi Y XR Origin
        Vector3 euler = transform.rotation.eulerAngles;
        float targetY = xrOrigin.rotation.eulerAngles.y;

        // Smooth interpolasi rotasi hanya di sumbu Y (putar badan)
        float smoothY = Mathf.LerpAngle(euler.y, targetY, Time.deltaTime * rotationSmoothSpeed);

        // Set rotasi baru dengan hanya ubah yaw
        transform.rotation = Quaternion.Euler(0, smoothY, 0);
    }
}
