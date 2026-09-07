using UnityEngine;
using UnityEngine.InputSystem;
public class player : MonoBehaviour
{
    #region variables
    [Header("Player Settings")]
    [SerializeField] private float speed = 20.0f;
    [SerializeField] private float jumpPower = 5.0f;
    [SerializeField] private float mouseSensitivity = 3f;
    
    [Header("Camera Settings")]
    [SerializeField] private GameObject playerCamera;
    private float camPitch;
    [Header("Oxygen Settings")]
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
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
#region Input Methods
    void OnSprint(InputValue value)
    {
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
            rb.AddForce(Vector3.down * 1.0f);// Reduced gravity force in water 
            camTest = transform.right * moveInput.x + playerCamera.transform.forward * moveInput.y;
            rb.AddForce(camTest.normalized * speed, ForceMode.Force);    
             if (jumpPressed)
            {
                rb.AddForce(Vector3.up * 2.5f, ForceMode.Force); // Apply upward force for swimming
            }
        }
        // normal submarine physics for the player
        if (isInAir)
        {
            // Apply normal physics
            rb.AddForce(Vector3.down * 9.81f, ForceMode.Force); // Normal gravity force
            
            Vector3 moveDirection = transform.right * moveInput.x + transform.forward * moveInput.y;
            
            rb.AddForce(moveDirection.normalized * speed, ForceMode.Force);
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