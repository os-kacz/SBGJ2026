using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 8f;
    public float lookSensitivity = 0.1f;
    public float jumpForce = 1f;
    public float gravity = -9.81f;

    private CharacterController controller;
    private Camera playerCamera;
    private float verticalRotation = 0f;
    private Vector3 velocity;

    // Crouching
    private float originalHeight;
    public float crouchHeight = 1f;
    public float crouchSpeed = 2.5f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();
        originalHeight = controller.height;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        transform.position = new Vector3(transform.position.x, 1, transform.position.z);
        // Controller & Mouse Look
        float lookX = InputSystem.actions["Look"].ReadValue<Vector2>().x * lookSensitivity;
        float lookY = InputSystem.actions["Look"].ReadValue<Vector2>().y * lookSensitivity;

        verticalRotation -= lookY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);
        playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        transform.Rotate(Vector3.up * lookX);

        // Jumping
        if(InputSystem.actions["Jump"].triggered && controller.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }

        // Crouching
        bool isCrouching = InputSystem.actions["Crouch"].ReadValue<float>() > 0.1f;
        controller.height = isCrouching ? crouchHeight : originalHeight;

        // Movement & Sprinting
        bool isSprinting = InputSystem.actions["Sprint"].ReadValue<float>() > 0.1f;
        float currentSpeed = isCrouching ? crouchSpeed : (isSprinting ? sprintSpeed : walkSpeed);

        float moveX = InputSystem.actions["Move"].ReadValue<Vector2>().x; // Automatically mapped to Left Stick / WASD
        float moveZ = InputSystem.actions["Move"].ReadValue<Vector2>().y;   // Automatically mapped to Left Stick / WASD

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        controller.Move(move * currentSpeed * Time.deltaTime);

        // Gravity
        if (controller.isGrounded && velocity.y < 0) velocity.y = -2f;
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}