// FirstPersonController.cs
// Ryan James Bernal

// Purpose: Provide the user the ability to navigate through the village

using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPersonModeController : MonoBehaviour
{
    [Header("Player References")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private CharacterController characterController;

    [Header("Player Settings")]
    [SerializeField] private float cameraSensitivity;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float moveInputDeadZone;

    // Touch Detection
    private int leftFingerId, rightFingerId;
    private float halfScreenWidth;

    // Camera Controls
    private Vector2 lookInput;
    private float cameraPitch;

    // Player Movement
    private Vector2 moveTouchStartPosition;
    private Vector2 moveInput;
    private float verticalVelocity;

    // Start is called before the first frame update
    void Start()
    {
        // Track the current status of each finger input
        leftFingerId = -1;
        rightFingerId = -1;

        // Define the half point of the screen
        halfScreenWidth = Screen.width / 2;

        // Calculate areas where input cannot be registered
        moveInputDeadZone = Mathf.Pow(Screen.height / moveInputDeadZone, 2);
    }

    void Update()
    {
        // Keep checking for Touch Inputs
        GetTouchInput();

        // Right Finger controls looking around
        if (rightFingerId != -1)
        {
            Move();
        }

        // Left Finger controls movement
        if (leftFingerId != -1)
        {
            LookAround();
        }
    }
    
    void GetTouchInput()
    {
        // Find a touchscreen
        if (Touchscreen.current == null)
        {
            return;
        }

        // Store a bool for the current status of each finger input
        bool leftFingerSeen = false;
        bool rightFingerSeen = false;

        // Check through each input found
        foreach (var touch in Touchscreen.current.touches)
        {
            var phase = touch.phase.ReadValue();

            if (phase == UnityEngine.InputSystem.TouchPhase.None)
            {
                continue;
            }

            // Get values from the touch input
            int fingerId = touch.touchId.ReadValue();
            Vector2 position = touch.position.ReadValue();
            Vector2 delta = touch.delta.ReadValue();
            
            // If the finger input matches the left finger input
            if (fingerId == leftFingerId)
            {
                leftFingerSeen = true;
                
                // Check if the finger has moved and change the look input vector
                if (phase == UnityEngine.InputSystem.TouchPhase.Moved)
                {
                    lookInput = delta * cameraSensitivity;
                }
                else if (phase == UnityEngine.InputSystem.TouchPhase.Stationary)
                {
                    lookInput = Vector2.zero;
                }
            }
            
            // If the finger input matches the right finger input
            if (fingerId == rightFingerId)
            {
                rightFingerSeen = true;
                
                // Check if the finger has started or stopped moving to adjust the move input
                if (phase == UnityEngine.InputSystem.TouchPhase.Moved ||
                    phase == UnityEngine.InputSystem.TouchPhase.Stationary)
                {
                    moveInput = position - moveTouchStartPosition;
                }
            }
            
            if (phase == UnityEngine.InputSystem.TouchPhase.Began)
            {
                if (position.x < halfScreenWidth && leftFingerId == -1)
                {
                    leftFingerId = fingerId;
                    leftFingerSeen = true;

                    lookInput = Vector2.zero;
                }
                else if (position.x >= halfScreenWidth && rightFingerId == -1)
                {
                    rightFingerId = fingerId;
                    rightFingerSeen = true;
                    moveTouchStartPosition = position;
                    moveInput = Vector2.zero;
                }
            }
            
            // Reset input vectors and states when touch is cancelled
            if (phase == UnityEngine.InputSystem.TouchPhase.Ended ||
            phase == UnityEngine.InputSystem.TouchPhase.Canceled)
            {
                if (fingerId == leftFingerId)
                {
                    leftFingerId = -1;
                    lookInput = Vector2.zero;
                    leftFingerSeen = false;
                }

                if (fingerId == rightFingerId)
                {
                    rightFingerId = -1;
                    moveInput = Vector2.zero;
                    rightFingerSeen = false;
                }
            }
        }
        
        if (leftFingerId != -1 && !leftFingerSeen)
        {
            leftFingerId = -1;
            lookInput = Vector2.zero;
        }

        if (rightFingerId != -1 && !rightFingerSeen)
        {
            rightFingerId = -1;
            moveInput = Vector2.zero;
        }
    }

    void LookAround() {

        // vertical (pitch) rotation
        cameraPitch = Mathf.Clamp(cameraPitch - lookInput.y, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(cameraPitch, 0, 0);

        // horizontal (yaw) rotation
        transform.Rotate(transform.up, lookInput.x);
    }
    
    void Move()
    {
        Vector3 movement = Vector3.zero;

        if (moveInput.sqrMagnitude > moveInputDeadZone)
        {
            Vector2 movementDirection = moveInput.normalized;

            // Use the headset camera's orientation,
            // projected onto the horizontal ground plane.
            Vector3 forward = Vector3.ProjectOnPlane(
                cameraTransform.forward,
                Vector3.up
            ).normalized;

            Vector3 right = Vector3.ProjectOnPlane(
                cameraTransform.right,
                Vector3.up
            ).normalized;

            // Fallback in case the camera is looking straight up or down
            if (forward.sqrMagnitude < 0.001f)
            {
                forward = Vector3.ProjectOnPlane(
                    transform.forward,
                    Vector3.up
                ).normalized;
            }

            if (right.sqrMagnitude < 0.001f)
            {
                right = Vector3.Cross(Vector3.up, forward).normalized;
            }

            movement =
                right * movementDirection.x +
                forward * movementDirection.y;

            // Prevent diagonal movement from being faster.
            movement = Vector3.ClampMagnitude(movement, 1f) * moveSpeed;
        }

        // Account for Gravity
        if (characterController.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }

        verticalVelocity += gravity * Time.deltaTime;

        // Combine horizontal movement and vertical velocity.
        movement.y = verticalVelocity;

        characterController.Move(movement * Time.deltaTime);
    }
}