using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
   /* [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip[] footstepSounds; // Array of footstep sounds to choose from
    public float walkInterval = 0.5f; // Time interval between footsteps when walking
    public float sprintInterval = 0.3f; // Time interval between footsteps when sprinting
    public float crouchInterval = 0.7f; // Time interval between footsteps when crouching
    public float walkVolume = 0.5f; // Volume for walking footsteps
    public float sprintVolume = 1f; // Volume for sprinting footsteps
    public float crouchVolume = 0.3f; // Volume for crouching footstep

    private float stepTimer = 0f; // Timer to track time between footsteps*/

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

    [SerializeField]
    private Rigidbody rb;

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
        bool isMoving = controller.velocity.magnitude > 0.1f && controller.isGrounded;

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

        // Footstep Sounds

       /* float currentInterval = walkInterval;
        float currentVolume = walkVolume;

        if (isSprinting)
        {
            currentInterval = sprintInterval;
            currentVolume = sprintVolume;
        }
        else if (isCrouching)
        {
            currentInterval = crouchInterval;
            currentVolume = crouchVolume;
        }
        
        if (isMoving)
        {
            stepTimer -= Time.deltaTime;

            if (stepTimer <= 0f)
            {
                PlayRandomFootStep(currentVolume);
                stepTimer = currentInterval;
            }
        }
        else
        {
            stepTimer = 0f;
        }*/


    }

    public void SetSlow(bool isSlowed)
    {
        if (isSlowed)
        {
            walkSpeed = 1.5f; // Reduced speed when slowed
            sprintSpeed = 3f; // Reduced sprint speed when slowed
        }
        else
        {
            walkSpeed = 5f; // Reset to normal speed
            sprintSpeed = 8f; // Reset to normal sprint speed
        }
    }

    public void ApplyKnockback(Vector3 explosionPoint)
    {
        rb.linearVelocity = Vector3.zero; // Reset current velocity
        rb.AddExplosionForce(1000f, explosionPoint, 200f, 100f, ForceMode.Impulse);
    }

    /*public void PlayRandomFootStep(float volume)
    {
        if (audioSource != null && footstepSounds.Length > 0)
        {
            int randomIndex = Random.Range(0, footstepSounds.Length);
            audioSource.pitch = Random.Range(0.8f, 1.2f); // Randomize pitch for variety
            audioSource.PlayOneShot(footstepSounds[randomIndex], volume);
        }
    }*/

}