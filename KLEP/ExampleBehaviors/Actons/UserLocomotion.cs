using System.Collections;
using UnityEngine;
using System.Collections;
using UnityEngine;

/// <summary>
/// UserLocomotion is a KLEP executable that manages character movement, including walking, dashing, 
/// jumping, double jumping, and flying. It integrates with KLEP's key-based input system to enable dynamic, modular control.
/// </summary>
public class UserLocomotion : KLEPExecutableBase
{
    [Header("Movement Settings")]
    [Tooltip("Walking speed of the character.")]
    public float moveSpeed = 5f;

    [Tooltip("Dashing speed of the character.")]
    public float dashSpeed = 10f;

    [Tooltip("Gravity force applied to the character.")]
    public float gravity = -9.81f;

    [Tooltip("Force applied when jumping.")]
    public float jumpForce = 5f;

    [Tooltip("Speed of character rotation.")]
    public float rotationSpeed = 4f;

    [Tooltip("Duration of the dash movement in seconds.")]
    public float dashDuration = 0.2f;

    [Tooltip("Maximum number of allowed double jumps.")]
    public int maxDoubleJumps = 1;

    [Tooltip("Enables flying mode, allowing vertical movement without gravity.")]
    public bool isFlying = false;

    [Header("Jump Configuration")]
    [Tooltip("Restrict movement to jump direction while airborne.")]
    public bool restrictMovementToJumpDirection = true;

    [Tooltip("Is the character's movement currently restricted to the jump direction?")]
    public bool isMovementRestricted = false;

    [Header("Input Keys")]
    [Tooltip("Key to move forward.")]
    public string forwardKey = "W_Hold";

    [Tooltip("Key to move backward.")]
    public string backKey = "S_Hold";

    [Tooltip("Key to move left.")]
    public string leftKey = "A_Hold";

    [Tooltip("Key to move right.")]
    public string rightKey = "D_Hold";

    [Tooltip("Key to dash forward.")]
    public string forwardDashKey = "W_DoubleTap";

    [Tooltip("Key to dash backward.")]
    public string backDashKey = "S_DoubleTap";

    [Tooltip("Key to dash left.")]
    public string leftDashKey = "A_DoubleTap";

    [Tooltip("Key to dash right.")]
    public string rightDashKey = "D_DoubleTap";

    [Tooltip("Key to initiate a jump.")]
    public string jumpKey = "Space_Down";

    [Tooltip("Key to activate flying mode.")]
    public string flyKey = "Space_Hold";

    [Header("References")]
    [Tooltip("Reference to the CharacterController component for movement.")]
    [SerializeField] private CharacterController controller;

    // Movement state variables
    private Vector3 velocity;
    private bool isDashing = false;
    private int remainingDoubleJumps;
    private bool wasGroundedLastFrame = false;

    // Dash management
    private Coroutine dashCoroutine;
    private Vector3 dashDirection = Vector3.zero;
    private Vector3 jumpDirection = Vector3.zero;

    [Header("Rotation Settings")]
    [Tooltip("If true, use MouseData_KEY for rotation.")]
    public bool useMouseDataKey = true;

    [Tooltip("Name of the key in the neuron's buffer for mouse data.")]
    public string mouseDataKeyName = "MouseData_KEY";

    /// <summary>
    /// Initialization method called when the executable is set up.
    /// Initializes jump counters and validates references.
    /// </summary>
    public override void Init()
    {
        base.Init();
        remainingDoubleJumps = maxDoubleJumps;

        // Attempt to find the CharacterController if not assigned
        if (!controller)
        {
            controller = GetComponent<CharacterController>();
            if (!controller)
            {
                Debug.LogError($"{executableName}: No CharacterController found on the object.");
            }
        }
    }

    /// <summary>
    /// ExecutableUpdates is called every frame.
    /// It handles all movement, including jumping, dashing, flying, and applying gravity.
    /// </summary>
    public override void ExecutableUpdates()
    {
        if (parentNeuron == null)
        {
            Debug.LogWarning($"{executableName}: No parent neuron detected. Skipping execution.");
            return;
        }

        HandleJump();    // Manages jumping and double-jumping
        HandleDash();    // Checks for dash input and initiates dashes
        HandleMovement(); // Applies standard movement
        HandleRotation(); // Manages character rotation

        ApplyGravityOrFly(); // Handles vertical movement based on gravity or flying mode

        Vector3 finalMovement = velocity * Time.deltaTime;

        // Adds dash movement if currently dashing
        if (isDashing)
        {
            finalMovement += dashDirection.normalized * dashSpeed * Time.deltaTime;
        }

        // Applies the combined movement to the character
        controller.Move(finalMovement);
    }

    /// <summary>
    /// Handles standard movement based on input keys.
    /// Takes into account movement restrictions during jumps.
    /// </summary>
    private void HandleMovement()
    {
        if (isMovementRestricted && !isDashing)
        {
            Vector3 restrictedMove = jumpDirection.normalized * moveSpeed * Time.deltaTime;
            controller.Move(restrictedMove);
            return;
        }

        Vector3 moveDirection = Vector3.zero;

        if (IsKeyPresent(forwardKey, KeyCheckType.inNeuron)) moveDirection += controller.transform.forward;
        if (IsKeyPresent(backKey, KeyCheckType.inNeuron)) moveDirection -= controller.transform.forward;
        if (IsKeyPresent(leftKey, KeyCheckType.inNeuron)) moveDirection -= controller.transform.right;
        if (IsKeyPresent(rightKey, KeyCheckType.inNeuron)) moveDirection += controller.transform.right;

        if (!isDashing)
        {
            Vector3 finalMove = moveDirection.normalized * moveSpeed;
            velocity.x = finalMove.x;
            velocity.z = finalMove.z;
        }
        else
        {
            velocity.x = 0;
            velocity.z = 0;
        }
    }

    /// <summary>
    /// Handles dash mechanics based on input keys.
    /// Only allows one dash at a time.
    /// </summary>
    private void HandleDash()
    {
        if (isDashing) return;

        Vector3 direction = Vector3.zero;

        if (IsKeyPresent(forwardDashKey, KeyCheckType.inNeuron)) direction = controller.transform.forward;
        if (IsKeyPresent(backDashKey, KeyCheckType.inNeuron)) direction = -controller.transform.forward;
        if (IsKeyPresent(leftDashKey, KeyCheckType.inNeuron)) direction = -controller.transform.right;
        if (IsKeyPresent(rightDashKey, KeyCheckType.inNeuron)) direction = controller.transform.right;

        if (direction != Vector3.zero)
        {
            StartCoroutine(DashCoroutine(direction));
        }
    }

    /// <summary>
    /// Coroutine to handle dash duration.
    /// Disables dashing after the set duration.
    /// </summary>
    /// <param name="direction">The direction of the dash movement.</param>
    private IEnumerator DashCoroutine(Vector3 direction)
    {
        isDashing = true;
        dashDirection = direction;
        Debug.Log($"Started dashing in direction: {direction}");

        float elapsedTime = 0f;

        while (elapsedTime < dashDuration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        isDashing = false;
        dashDirection = Vector3.zero;
        Debug.Log("Finished dashing.");
    }

    /// <summary>
    /// Handles jump and double jump mechanics, as well as flying mode when enabled.
    /// Supports movement direction control during jumps and manages movement restrictions.
    /// </summary>
    private void HandleJump()
    {
        bool isGrounded = controller.isGrounded; // Use CharacterController's isGrounded property

        // Reset double jumps only when landing (transition from not grounded to grounded)
        if (isGrounded && !wasGroundedLastFrame)
        {
            remainingDoubleJumps = maxDoubleJumps;

            // Reset jump direction and remove movement restrictions
            jumpDirection = Vector3.zero;
            isMovementRestricted = false;
        }

        wasGroundedLastFrame = isGrounded; // Update grounded state for the next frame

        if (isFlying)
        {
            // Handle vertical movement when in flying mode
            if (IsKeyPresent(flyKey, KeyCheckType.inNeuron))
            {
                velocity.y = Mathf.Lerp(velocity.y, jumpForce, Time.deltaTime); // Smooth upward motion
            }
            else
            {
                velocity.y = Mathf.Lerp(velocity.y, 0f, Time.deltaTime); // Smooth downward motion
            }
        }
        else
        {
            // Handle jump input when not in flying mode
            if (IsKeyPresent(jumpKey, KeyCheckType.inNeuron))
            {
                Debug.Log("Jump key detected.");

                // Allow jumping if grounded or if double jumps are available
                if (isGrounded || remainingDoubleJumps > 0)
                {
                    Vector3 inputDirection = Vector3.zero;

                    // Determine jump direction based on current input keys
                    if (IsKeyPresent(forwardKey, KeyCheckType.inNeuron)) inputDirection += controller.transform.forward;
                    if (IsKeyPresent(backKey, KeyCheckType.inNeuron)) inputDirection -= controller.transform.forward;
                    if (IsKeyPresent(leftKey, KeyCheckType.inNeuron)) inputDirection -= controller.transform.right;
                    if (IsKeyPresent(rightKey, KeyCheckType.inNeuron)) inputDirection += controller.transform.right;

                    // Set jump direction or default to forward if no input is given
                    jumpDirection = inputDirection != Vector3.zero ? inputDirection.normalized : controller.transform.forward;

                    // Apply the jump force to vertical velocity
                    velocity.y = jumpForce;

                    // Manage movement restriction during the initial jump
                    if (isGrounded)
                    {
                        isMovementRestricted = restrictMovementToJumpDirection;
                        Debug.Log($"Movement restriction set to: {isMovementRestricted}");
                    }

                    // Handle double jump logic
                    if (!isGrounded)
                    {
                        remainingDoubleJumps--;

                        if (restrictMovementToJumpDirection && inputDirection != Vector3.zero)
                        {
                            jumpDirection = inputDirection.normalized;
                        }
                    }
                }
                else
                {
                    Debug.Log("Jumping not allowed. Either not grounded or no double jumps left.");
                }
            }
        }
    }

    /// <summary>
    /// Applies gravity when not flying, or manages vertical motion in fly mode.
    /// Ensures consistent behavior when transitioning between grounded and air states.
    /// </summary>
    private void ApplyGravityOrFly()
    {
        if (isFlying)
        {
            if (IsKeyPresent(flyKey, KeyCheckType.inNeuron))
            {
                velocity.y = Mathf.Lerp(velocity.y, jumpForce, Time.deltaTime); // Smooth upward motion
            }
            else
            {
                velocity.y = Mathf.Lerp(velocity.y, 0f, Time.deltaTime); // Smooth downward motion
            }
        }
        else
        {
            if (controller.isGrounded && velocity.y < 0)
            {
                velocity.y = -2f; // Small negative value to keep the controller grounded
            }

            velocity.y += gravity * Time.deltaTime; // Apply gravity when not flying
        }
    }

    /// <summary>
    /// Manages the character's rotation based on mouse input.
    /// Supports both MouseData_KEY input and screen mouse position.
    /// </summary>
    private void HandleRotation()
    {
        if (useMouseDataKey)
        {
            UseMouseDataKeyForRotation();
        }
        else
        {
            UseScreenMousePositionForRotation();
        }
    }

    /// <summary>
    /// Rotates the character towards the mouse position on screen.
    /// Useful for top-down or third-person perspectives where the character
    /// should face the mouse cursor.
    /// </summary>
    private void UseScreenMousePositionForRotation()
    {
        if (Camera.main == null)
        {
            Debug.LogError($"{executableName}: Main Camera is not assigned. Rotation functionality will not work.");
            return;
        }

        Vector3 mouseScreenPosition = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mouseScreenPosition);

        Plane groundPlane = new Plane(Vector3.up, new Vector3(0, controller.transform.position.y, 0));
        if (groundPlane.Raycast(ray, out float rayDistance))
        {
            Vector3 targetPoint = ray.GetPoint(rayDistance);
            Vector3 directionToTarget = targetPoint - controller.transform.position;
            directionToTarget.y = 0f;

            if (directionToTarget.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
                controller.transform.rotation = Quaternion.Lerp(controller.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
    }

    /// <summary>
    /// Uses the MouseData_KEY from the neuron to control rotation.
    /// Allows integration with external systems that provide mouse world positions.
    /// </summary>
    private void UseMouseDataKeyForRotation()
    {
        if (parentNeuron.keyManager.GetKey(mouseDataKeyName, out KLEPKey mouseDataKey)
            && mouseDataKey.HasProperty("MouseWorldPosition"))
        {
            Vector3 targetPosition = mouseDataKey.GetProperty<Vector3>("MouseWorldPosition");
            Vector3 directionToTarget = targetPosition - controller.transform.position;
            directionToTarget.y = 0f;

            if (directionToTarget.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
                controller.transform.rotation = Quaternion.Lerp(controller.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
        else
        {
            Debug.LogWarning($"{executableName}: MouseData_KEY or MouseWorldPosition not found.");
        }
    }

    /// <summary>
    /// Cleans up active coroutines and resets movement flags when the executable is reset.
    /// Ensures proper state management and avoids lingering effects from dashes or jumps.
    /// </summary>
    public override void Cleanup()
    {
        isDashing = false;
        dashDirection = Vector3.zero;
        velocity = Vector3.zero;
        jumpDirection = Vector3.zero;

        if (dashCoroutine != null)
        {
            StopCoroutine(dashCoroutine);
            dashCoroutine = null;
        }

        Debug.Log($"{executableName}: Cleanup completed.");
    }
}

