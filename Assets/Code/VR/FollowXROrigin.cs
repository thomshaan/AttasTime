using UnityEngine;

public class FollowXROrigin : MonoBehaviour
{
    //public Transform xrOrigin;
    //public float rotationSmoothSpeed = 5f;

    //void LateUpdate()
    //{
    //    if (xrOrigin == null) return;

    //    // Ikuti posisi langsung
    //    transform.position = xrOrigin.position;

    //    // Ambil rotasi Y XR Origin
    //    Vector3 euler = transform.rotation.eulerAngles;
    //    float targetY = xrOrigin.rotation.eulerAngles.y;

    //    // Smooth interpolasi rotasi hanya di sumbu Y (putar badan)
    //    float smoothY = Mathf.LerpAngle(euler.y, targetY, Time.deltaTime * rotationSmoothSpeed);

    //    // Set rotasi baru dengan hanya ubah yaw
    //    transform.rotation = Quaternion.Euler(0, smoothY, 0);
    //}
    public Transform cameraOffset; // Biasanya CameraOffset atau Main Camera
    public float rotationSmoothSpeed = 5f;

    private Vector3 lastPosition;
    private Animator animator;

    void Start()
    {
        lastPosition = cameraOffset.position;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // ----------------------------
        // 1. FOLLOW POSISI KAMERA (XZ)
        // ----------------------------
        Vector3 direction = cameraOffset.forward;
        direction.y = 0f;
        direction.Normalize();

        // Atur seberapa jauh karakter dari kamera
        float distanceBehind = 0.2f;
        Vector3 offsetPosition = cameraOffset.position - direction * distanceBehind;

        // Tetap jaga posisi Y karakter agar tidak melayang atau masuk ke tanah
        Vector3 targetPosition = new Vector3(offsetPosition.x, transform.position.y, offsetPosition.z);
        transform.position = targetPosition;


        // ----------------------------
        // 2. FOLLOW ROTASI KAMERA (Y only)
        // ----------------------------
        Vector3 lookDirection = cameraOffset.forward;
        lookDirection.y = 0f;

        if (lookDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSmoothSpeed);
        }

        lastPosition = cameraOffset.position;
    }

}
