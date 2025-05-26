using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class ThirdPersonController : MonoBehaviour
{
    public float velocity = 5f;
    public float sprintAdittion = 3.5f;
    public float jumpForce = 18f;
    public float jumpTime = 0.85f;
    public float gravity = 9.8f;

    float jumpElapsedTime = 0;
    bool isJumping = false;
    bool isSprinting = false;
    bool isCrouching = false;
    Vector2 moveInput;
    bool inputJump;
    bool inputSprint;
    bool inputCrouch;
    Animator animator;
    CharacterController cc;

    private bool forceForwardInput = false;
    private bool canMove = true;
    private bool skipNextFixedUpdate = false;

    private void Awake()
    {
    }

    void Start()
    {
        cc = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        if (animator == null)
            Debug.LogWarning("Animator component not found on player.");
    }

    void Update()
    {
        if (!canMove)
        {
            moveInput = Vector2.zero;
            inputJump = false;
            inputSprint = false;
            inputCrouch = false;
            return;
        }

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

        if (cc.isGrounded && animator != null)
        {
            animator.SetBool("run", cc.velocity.magnitude > 0.9f);
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

        HeadHittingDetect();
    }

    void FixedUpdate()
    {
        if (skipNextFixedUpdate)
        {
            skipNextFixedUpdate = false; // reset flag, skip movement sekali saja
            return;
        }

        float velocityAdittion = isSprinting ? sprintAdittion : (isCrouching ? -(velocity * 0.50f) : 0);

        float directionX = moveInput.x * (velocity + velocityAdittion) * Time.deltaTime;
        float directionZ = moveInput.y * (velocity + velocityAdittion) * Time.deltaTime;
        float directionY = 0;

        // Jump logic (sesuaikan sesuai kode asli mu)
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

        // --- Character rotation ---
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
        cc.Move(movement);
    }

    public void SkipMovementNextFrame()
    {
        skipNextFixedUpdate = true;
    }

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

    public void SetForceForward(bool enabled)
    {
        forceForwardInput = enabled;
    }
}
