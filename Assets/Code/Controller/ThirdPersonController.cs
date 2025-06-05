using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class ThirdPersonController : MonoBehaviour
{
    public float velocity = 5f;
    public float sprintAddition = 3.5f;
    public float jumpForce = 18f;
    public float jumpTime = 0.85f;
    public float gravity = 9.8f;

    // Reference to AttaAnimator (AnimHandler)
    public AttaAnimator animHandler;

    private float jumpElapsedTime = 0;
    private bool isJumping = false;
    private bool isSprinting = false;
    private bool isCrouching = false;
    private Vector2 moveInput;
    private bool inputJump;
    private bool inputSprint;
    private bool inputCrouch;
    private CharacterController cc;

    private bool forceForwardInput = false;
    private bool canMove = true;
    private bool skipNextFixedUpdate = false;

    void Start()
    {
        cc = GetComponent<CharacterController>();
        if (animHandler == null)
            animHandler = GetComponent<AttaAnimator>();  // Ensure AttaAnimator is properly referenced

        if (animHandler == null)
            Debug.LogWarning("AnimHandler (AttaAnimator) component not found on player.");

        if (cc == null)
            Debug.LogWarning("CharacterController not found.");
    }

    void Update()
    {
        // If movement is locked (dialog active), ignore input and stop all movement.
        if (!canMove)
        {
            Debug.Log("[ThirdPersonController] Movement input blocked.");

            // Clear all movement-related inputs
            moveInput = Vector2.zero;  // Reset movement input
            inputJump = false;         // Reset jump input
            inputSprint = false;       // Reset sprint input
            inputCrouch = false;       // Reset crouch input

            // Explicitly stop movement
            cc.Move(Vector3.zero);  // Stop movement

            // Ensure the character is in idle state
            animHandler.SetBool("jalan", false);  // Set "jalan" animation to false (idle)

            return; // Skip the rest of the movement logic
        }

        // Handle input if movement is allowed
        if (Gamepad.current != null)
        {
            moveInput = Gamepad.current.leftStick.ReadValue();
            inputJump = Gamepad.current.buttonSouth.wasPressedThisFrame;
            inputSprint = Gamepad.current.buttonWest.isPressed;
            inputCrouch = Gamepad.current.buttonEast.wasPressedThisFrame;
        }

        moveInput.x = moveInput.x != 0 ? moveInput.x : Input.GetAxis("Horizontal");
        moveInput.y = moveInput.y != 0 ? moveInput.y : Input.GetAxis("Vertical");
        inputJump |= Input.GetKeyDown(KeyCode.Space);
        inputSprint |= Input.GetKey(KeyCode.LeftShift);
        inputCrouch |= Input.GetKeyDown(KeyCode.LeftControl);

        if (inputCrouch)
            isCrouching = !isCrouching;

        // Debug: Log when movement input is detected
        if (moveInput != Vector2.zero)
        {
            Debug.Log("[ThirdPersonController] Movement detected: " + moveInput);
        }

        // Handle grounded check and movement animation
        if (cc.isGrounded)
        {
            animHandler.SetBool("jalan", cc.velocity.magnitude > 0.9f);  // "jalan" -> movement animation
            isSprinting = cc.velocity.magnitude > 0.9f && inputSprint;
        }

        if (inputJump && cc.isGrounded)
        {
            isJumping = true;
        }

        if (forceForwardInput)
        {
            moveInput.y = 1f;
        }
        else
        {
            moveInput.y = moveInput.y != 0 ? moveInput.y : Input.GetAxis("Vertical");
        }

        // Call head hitting detection
        HeadHittingDetect();
    }

    void FixedUpdate()
    {
        if (skipNextFixedUpdate)
        {
            skipNextFixedUpdate = false; // Skip movement once
            return;
        }

        if (!canMove)
        {
            Debug.Log("[ThirdPersonController] Movement is halted in FixedUpdate.");

            // Zero out movement completely and make sure the character doesn't move
            cc.Move(Vector3.zero);  // This stops the movement

            // Make sure animator shows idle animation
            animHandler.SetBool("jalan", false);  // "jalan" -> idle animation
            return; // No movement when blocked by dialog
        }

        // Movement calculations and application (if allowed)
        float velocityAddition = isSprinting ? sprintAddition : (isCrouching ? -(velocity * 0.50f) : 0);
        float directionX = moveInput.x * (velocity + velocityAddition) * Time.deltaTime;
        float directionZ = moveInput.y * (velocity + velocityAddition) * Time.deltaTime;
        float directionY = 0;

        // Jump logic (adjust as needed)
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

        // Character rotation
        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;

        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        forward *= directionZ;
        right *= directionX;

        if (directionX != 0 || directionZ != 0)
        {
            float angle = Mathf.Atan2(forward.x + right.x, forward.z + right.z) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.Euler(0, angle, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.15f);
        }

        Vector3 movement = Vector3.up * directionY + forward + right;
        cc.Move(movement);  // Move character
    }




    // Skips next fixed update
    public void SkipMovementNextFrame()
    {
        skipNextFixedUpdate = true;
    }

    // Detect head hitting objects (if needed)
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

    // Teleportation logic (if needed)
    public void TeleportToPosition(Vector3 newPosition, float rotationY)
    {
        StartCoroutine(TeleportRoutine(newPosition, rotationY));
    }

    private IEnumerator TeleportRoutine(Vector3 newPosition, float rotationY)
    {
        canMove = false;

        var controller = GetComponent<CharacterController>();
        if (controller != null)
        {
            controller.enabled = false;
        }

        transform.position = newPosition;
        transform.rotation = Quaternion.Euler(0, rotationY, 0);

        isJumping = false;
        jumpElapsedTime = 0;

        yield return null;

        if (controller != null)
        {
            controller.enabled = true;
        }

        canMove = true;

        yield break;
    }

    // Force forward input (if needed)
    public void SetForceForward(bool enabled)
    {
        forceForwardInput = enabled;
    }

    // Halts all movement and sets idle animation
    public void HaltMovement()
    {
        Debug.Log("[ThirdPersonController] Movement is halted. No more input should be processed.");

        // Clear movement input
        moveInput = Vector2.zero;
        inputJump = false;
        inputSprint = false;
        inputCrouch = false;
        isJumping = false;
        jumpElapsedTime = 0;

        // Stop CharacterController's residual movement
        cc.Move(Vector3.zero);  // Zero movement

        // Ensure animator shows idle animation
        animHandler.SetBool("jalan", false);  // Use animHandler to set "jalan" to false (idle)
    }
}
