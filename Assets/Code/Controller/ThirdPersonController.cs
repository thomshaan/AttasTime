using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class ThirdPersonController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float velocity = 5f;
    public float sprintAddition = 3.5f;
    public float jumpForce = 18f;
    public float jumpTime = 0.85f;
    public float gravity = 9.8f;

    [Header("Input")]
    public InputActionReference moveAction;

    [Header("Animation")]
    public AttaAnimator animHandler;

    private CharacterController cc;
    private Vector2 moveInput;
    private bool isSprinting = false;
    private bool forceForwardInput = false;
    private bool canMove = true;
    private bool skipNextFixedUpdate = false;

    private float jumpElapsedTime = 0;
    private bool isJumping = false;
    private bool isCrouching = false;
    private bool inputJump;
    private bool inputSprint;
    private bool inputCrouch;

    void Start()
    {
        cc = GetComponent<CharacterController>();
        if (animHandler == null)
            animHandler = GetComponent<AttaAnimator>();

        moveAction?.action.Enable();

        if (cc == null) Debug.LogWarning("CharacterController missing.");
        if (animHandler == null) Debug.LogWarning("AttaAnimator missing.");
    }

    void Update()
    {
        if (!canMove)
        {
            moveInput = Vector2.zero;
            cc.Move(Vector3.zero);
            animHandler?.SetBool("jalan", false);
            animHandler?.SetBool("lari", false);
            return;
        }

        moveInput = moveAction.action.ReadValue<Vector2>();

        if (forceForwardInput)
            moveInput.y = 1f;

        isSprinting = moveInput.magnitude > 0.8f;

        animHandler?.SetBool("jalan", moveInput.magnitude > 0.1f);
        animHandler?.SetBool("lari", isSprinting);

        // Handle input for jumping and crouching
        inputJump = Input.GetKeyDown(KeyCode.Space);
        inputSprint = Input.GetKey(KeyCode.LeftShift);
        inputCrouch = Input.GetKeyDown(KeyCode.LeftControl);

        if (inputCrouch)
            isCrouching = !isCrouching;

        // Jump logic
        if (inputJump && cc.isGrounded)
        {
            isJumping = true;
        }

        // Head hitting detection
        HeadHittingDetect();
    }

    void FixedUpdate()
    {
        if (skipNextFixedUpdate)
        {
            skipNextFixedUpdate = false;
            return;
        }

        if (!canMove)
        {
            cc.Move(Vector3.zero);
            animHandler?.SetBool("jalan", false);
            animHandler?.SetBool("lari", false);
            return;
        }

        // Movement calculations
        float velocityAddition = isSprinting ? sprintAddition : (isCrouching ? -(velocity * 0.50f) : 0);
        float directionX = moveInput.x * (velocity + velocityAddition) * Time.deltaTime;
        float directionZ = moveInput.y * (velocity + velocityAddition) * Time.deltaTime;
        float directionY = 0;

        // Jump logic
        if (isJumping)
        {
            directionY = Mathf.SmoothStep(jumpForce, jumpForce * 0.30f, jumpElapsedTime / jumpTime) * Time.deltaTime;
            jumpElapsedTime += Time.deltaTime;
            if (jumpElapsedTime >= jumpTime)
            {
                isJumping = false;
                jumpElapsedTime = 0;
            }
        }

        directionY -= gravity * Time.deltaTime;

        // Character rotation based on camera orientation
        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;

        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        // Calculate movement direction relative to the camera
        forward *= directionZ;
        right *= directionX;

        if (directionX != 0 || directionZ != 0)
        {
            float angle = Mathf.Atan2(forward.x + right.x, forward.z + right.z) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.Euler(0, angle, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.15f); // Smooth rotation
        }

        // Apply movement to the character
        Vector3 movement = Vector3.up * directionY + forward + right;
        cc.Move(movement);  // Move the character
    }

    // Skips next fixed update (useful for halting movement temporarily)
    public void SkipMovementNextFrame()
    {
        skipNextFixedUpdate = true;
    }

    // Detects if the character hits an object with its head (e.g., ceiling)
    void HeadHittingDetect()
    {
        float headHitDistance = 1.1f;
        Vector3 ccCenter = transform.TransformPoint(cc.center);
        float hitCalc = cc.height / 2f * headHitDistance;

        if (Physics.Raycast(ccCenter, Vector3.up, hitCalc))
        {
            jumpElapsedTime = 0;
            isJumping = false;
        }
    }

    // Teleport the character to a new position with rotation
    public void TeleportToPosition(Vector3 newPosition, float rotationY)
    {
        StartCoroutine(TeleportRoutine(newPosition, rotationY));
    }

    private IEnumerator TeleportRoutine(Vector3 newPosition, float rotationY)
    {
        canMove = false;
        cc.enabled = false;

        transform.position = newPosition;
        transform.rotation = Quaternion.Euler(0, rotationY, 0);

        yield return null;

        cc.enabled = true;
        canMove = true;
    }

    // Force the player to move forward regardless of input
    public void SetForceForward(bool enabled)
    {
        forceForwardInput = enabled;
    }

    // Halt all movement and set idle animation
    public void HaltMovement()
    {
        moveInput = Vector2.zero;
        cc.Move(Vector3.zero);
        animHandler?.SetBool("jalan", false);
        animHandler?.SetBool("lari", false);
    }
}
