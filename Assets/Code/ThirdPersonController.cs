using UnityEngine;
using UnityEngine.InputSystem; // Import Input System

public class ThirdPersonController : MonoBehaviour
{
    [Tooltip("Speed ​​at which the character moves. It is not affected by gravity or jumping.")]
    public float velocity = 5f;
    [Tooltip("This value is added to the speed value while the character is sprinting.")]
    public float sprintAdittion = 3.5f;
    [Tooltip("The higher the value, the higher the character will jump.")]
    public float jumpForce = 18f;
    [Tooltip("Stay in the air. The higher the value, the longer the character floats before falling.")]
    public float jumpTime = 0.85f;
    [Space]
    [Tooltip("Force that pulls the player down. Changing this value causes all movement, jumping and falling to be changed as well.")]
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

    private void Awake()
    {


    }

    void Start()
    {
        cc = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        if (animator == null)
            Debug.LogWarning("Hey buddy, you don't have the Animator component in your player. Without it, the animations won't work.");
    }

    void Update()
    {
        // Gamepad input handling
        if (Gamepad.current != null)
        {
            moveInput = Gamepad.current.leftStick.ReadValue();
            inputJump = Gamepad.current.buttonSouth.wasPressedThisFrame; // A (Xbox) / X (PS)
            inputSprint = Gamepad.current.buttonWest.isPressed; // X (Xbox) / Square (PS)
            inputCrouch = Gamepad.current.buttonEast.wasPressedThisFrame; // B (Xbox) / Circle (PS)
        }

        // Keyboard inputs as fallback
        moveInput.x = moveInput.x != 0 ? moveInput.x : Input.GetAxis("Horizontal");
        moveInput.y = moveInput.y != 0 ? moveInput.y : Input.GetAxis("Vertical");
        inputJump |= Input.GetKeyDown(KeyCode.Space);
        inputSprint |= Input.GetKey(KeyCode.LeftShift);
        inputCrouch |= Input.GetKeyDown(KeyCode.LeftControl);

        // Toggle crouch state
        if (inputCrouch)
            isCrouching = !isCrouching;

        // Handle animations
        if (cc.isGrounded && animator != null)
        {
            animator.SetBool("run", cc.velocity.magnitude > 0.9f);
            isSprinting = cc.velocity.magnitude > 0.9f && inputSprint;
        }

        // Jump logic
        if (inputJump && cc.isGrounded)
        {
            isJumping = true;
        }

        HeadHittingDetect();
    }

    void FixedUpdate()
    {
        float velocityAdittion = isSprinting ? sprintAdittion : (isCrouching ? -(velocity * 0.50f) : 0);

        float directionX = moveInput.x * (velocity + velocityAdittion) * Time.deltaTime;
        float directionZ = moveInput.y * (velocity + velocityAdittion) * Time.deltaTime;
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

        // Move the character
        Vector3 movement = Vector3.up * directionY + forward + right;
        cc.Move(movement);
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
}
