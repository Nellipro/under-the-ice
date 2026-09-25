using UnityEngine;
using UnityEngine.InputSystem;
public class player : MonoBehaviour
{
    #region variables
    [Header("Player Settings")]
    [SerializeField] private float speed = 20.0f;
    [SerializeField] private float jumpPower = 5.0f;
    [SerializeField] private float mouseSensitivity = 3f;

    [Header("Walking")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float groundAcceleration = 35f;
    [SerializeField] private float groundBraking = 45f;
    [SerializeField] private float airAcceleration = 8f;
    private bool sprintHeld;
    
    [Header("Camera Settings")]
    [SerializeField] private GameObject playerCamera;
    private float camPitch;
    [Header("Oxygen Settings")]
    //[SerializeField] private GameObject UnderwaterEffect;
    [SerializeField] private float oxygen = 100f;
    [SerializeField] private int maxOxygen = 100; // Maximum oxygen level

    [Header("UI Settings")]
    [SerializeField] private playerUI playerUI; // Reference to the playerUI script
   
    [Header("Environment State")]
    [SerializeField] private bool isInAir = true;
    bool freezeCam = false;

    [Header("Input State")]
    [SerializeField] private Vector2 moveInput;
    [SerializeField] private Vector2 viewInput;
    [SerializeField] private bool jumpPressed = false;
    [SerializeField] private bool jumpIsAllowed = true; // Flag to control jump permission
    [SerializeField] private bool crouchPressed = false;

    [Header("Other Settings")]

    private Vector3 camTest;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform groundCheckPos;
    #endregion
#region Input Methods
    void OnSprint(InputValue value)
    {
        sprintHeld = value.isPressed;
        if (value.isPressed)
        {
            if (isInAir) // Only allow sprinting if not in air
            {
                speed = 35.0f; // Increase speed when sprinting
            } else if (!isInAir)
            {
                speed = 15.0f; // Increase speed when sprinting
            }
        }
        else if (!value.isPressed)
        {
             if (isInAir) // Only allow sprinting if not in air
            {
                speed = 20.0f; // Reset speed when not sprinting
            } else if (!isInAir)
            {
                speed = 9.0f; // Reset speed when not sprinting
            } 
        }
    }
       
       void OnSwimUp(InputValue value)
    {
        if (value.isPressed)
        {
            jumpPressed = true; // Set jumpPressed to true when swim up is pressed
        }
        else if (!value.isPressed)
        {
            jumpPressed = false; // Set jumpPressed to false when swim up is released
        }
       
    }
    void OnJump(InputValue value)
    {
        if (isInAir && jumpIsAllowed) // Allow jumping if in air
        {
            rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse); // Apply upward force for jumping

        }
    }
    void OnCrouch(InputValue value)
    {
        if (!isInAir) // Only allow crouching if not in air
        {
        rb.AddForce(Vector3.down * 1.5f, ForceMode.Force); // Apply downward force for crouching
        }
    }
    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }
    void OnLook(InputValue value)
    {
        viewInput = value.Get<Vector2>();
        viewInput *= mouseSensitivity * 0.1f; // Adjust sensitivity as needed
    }
    #endregion
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Invoke(nameof(FixPlayer), 10f);
    }
    void FixPlayer()
    {
        moveInput = new Vector2(0, 0);
    }

  #region AirCheck
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Air"))
        {
            isInAir = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Air"))
        {
            isInAir = false;
        }
    }
    #endregion
    void Update()
    {
        freezeCam = playerUI.ShowUI;
        //raycast beneath players feet to check if they are in air or not
        jumpIsAllowed = Physics.Raycast(groundCheckPos.position, Vector3.down, 0.25f);

    }
    void FixedUpdate()
    {
        #region movement
    
        if (!isInAir)
        {
            
            // Apply water physics
            // For example, reduce gravity
            rb.AddForce(Vector3.down * 0.5f);// Reduced gravity force in water 
            camTest = transform.right * moveInput.x + playerCamera.transform.forward * moveInput.y;
            rb.AddForce(camTest.normalized * speed, ForceMode.Force);    
             if (jumpPressed)
            {
                rb.AddForce(Vector3.up * 2.5f, ForceMode.Force); // Apply upward force for swimming
            }
           //UnderwaterEffect.SetActive(true);
        }
        // normal submarine physics for the player
        if (isInAir)
        {
            //UnderwaterEffect.SetActive(false);
            // Apply normal physics
            rb.AddForce(Vector3.down * 9.81f, ForceMode.Force); // Normal gravity force
            
            Vector3 moveDirection = transform.right * moveInput.x + transform.forward * moveInput.y;
            moveDirection.y = 0f;
            moveDirection = Vector3.ClampMagnitude(moveDirection, 1f);

            float targetSpeed = sprintHeld ? sprintSpeed : walkSpeed;
            Vector3 targetVelocity = moveDirection * targetSpeed;

            Vector3 horizontalVelocity = rb.linearVelocity;
            horizontalVelocity.y = 0f;

            bool hasInput = moveInput.sqrMagnitude > 0.001f;
            bool grounded = jumpIsAllowed;

            // Brake on the ground; preserve airborne momentum without input.
            if (grounded || hasInput)
            {
                float acceleration = grounded
                    ? (hasInput ? groundAcceleration : groundBraking)
                    : airAcceleration;

                Vector3 nextVelocity = Vector3.MoveTowards(
                    horizontalVelocity,
                    targetVelocity,
                    acceleration * Time.fixedDeltaTime
                );

                // Adjust horizontal motion without changing jump/fall velocity.
                rb.AddForce(
                    nextVelocity - horizontalVelocity,
                    ForceMode.VelocityChange
                );
            }
        }
        if(!freezeCam) //camera system
        {
            // Rotate the camera based on view input
            gameObject.transform.Rotate(0f, viewInput.x, 0f);
            
            camPitch -= viewInput.y;
            camPitch = Mathf.Clamp(camPitch, -80f, 75f);
            playerCamera.transform.localRotation = Quaternion.Euler(camPitch, 0f, 0f);
        }

    
        #endregion
    }
    #region Oxygen
    void ChangeOxygen(float amount)//void for changing oxygen level without going above max or below 0.
    {
        oxygen = Mathf.Clamp(oxygen + amount, 0f, maxOxygen);
    }
    #endregion
}
