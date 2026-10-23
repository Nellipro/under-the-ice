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
    [SerializeField] private LayerMask JumpDetectionMask;
    private bool sprintHeld;
    [Header("Swimming")]
    [SerializeField] private float swimSpeed = 5f;
    [SerializeField] private float swimAcceleration = 10f;




    [Header("Camera Settings")]
    [SerializeField] private GameObject playerCamera;
    [SerializeField] private GameObject SubOverlayCamera;
    private float camPitch;
    [Header("Oxygen Settings")]
    //[SerializeField] private GameObject UnderwaterEffect;
    [SerializeField] private float oxygen = 100f;
    [SerializeField] private int maxOxygen = 100; // Maximum oxygen level

    [Header("UI Settings")]
    [SerializeField] private playerUI playerUI; // Reference to the playerUI script
   
    [Header("Environment State")]
    [SerializeField] private bool isInAir = true;
    public bool IsInAir => isInAir;
    bool freezeCam = false;

    [Header("Input State")]
    [SerializeField] private Vector2 moveInput;
    [SerializeField] private Vector2 viewInput;
    [SerializeField] private bool jumpPressed = false;
    [SerializeField] private bool jumpIsAllowed = true; // Flag to control jump permission
    [SerializeField] private bool crouchPressed = false;

    [Header("Other Settings")]

    private Vector3 swimDirection;
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
                speed = 35.0f;
                walkSpeed = 10f; // Increase speed when sprinting
            } else if (!isInAir)
            {
                swimSpeed = 10; // Increase speed when sprinting
            }
        }
        else if (!value.isPressed)
        {
             if (isInAir) // Only allow sprinting if not in air
            {
                speed = 20.0f;
                walkSpeed = 5f; // Reset speed when not sprinting
            } else if (!isInAir)
            {
                swimSpeed = 5; // Reset speed when not sprinting
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
        // Verandert het masker naar: "Raak alles, BEHALVE de laag die Player heet"
        JumpDetectionMask = ~LayerMask.GetMask("Player");

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
        //raycast beneath players feet to check if they are in air or not
        RaycastHit Hit;
        jumpIsAllowed = Physics.SphereCast(groundCheckPos.position, 0.5f, Vector3.down, out Hit, 1.15f, JumpDetectionMask);
        Debug.DrawRay(groundCheckPos.position, Vector3.down * 1.15f, jumpIsAllowed ? Color.green : Color.red);
        freezeCam = playerUI.ShowUI;
        if (isInAir)
        {
            SubOverlayCamera.SetActive(true);
        }
        else
        {
            SubOverlayCamera.SetActive(false);
        }
    }
    void FixedUpdate()
    {
        #region movement
    
        if (!isInAir)
        {
            
            // Apply water physics
            // For example, reduce gravity
            rb.AddForce(Vector3.down * 10f);// Reduced gravity force in water 

            swimDirection = transform.right * moveInput.x + playerCamera.transform.forward * moveInput.y;
            swimDirection = Vector3.ClampMagnitude(swimDirection, 1f);

            Vector3 targetVelocity = swimDirection.normalized * swimSpeed;
            Vector3 nextVelocity = Vector3.MoveTowards(
            rb.linearVelocity, targetVelocity, swimAcceleration * Time.fixedDeltaTime);

            rb.AddForce(nextVelocity - rb.linearVelocity, ForceMode.VelocityChange);



             if (jumpPressed)
            {
                rb.AddForce(Vector3.up * 30f, ForceMode.Force); // Apply upward force for swimming
            }
            if (crouchPressed)
            {
                rb.AddForce(Vector3.down * 30f, ForceMode.Force); // Apply downward force for swimming
            }
            
           
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
            bool grounded = jumpIsAllowed;
            Vector3 parentVelocity = Vector3.zero;

            // Start at the parent so we never select the player's own Rigidbody.
            if (grounded && transform.parent != null)
            {
                Rigidbody parentRb = transform.parent.GetComponentInParent<Rigidbody>();
                if (parentRb != null)
                {
                    parentVelocity = parentRb.GetPointVelocity(rb.position);
                    parentVelocity.y = 0f;
                }
            }

            // Standing still matches the submarine; walking adds relative velocity.
            Vector3 targetVelocity = parentVelocity + moveDirection * targetSpeed;

            Vector3 horizontalVelocity = rb.linearVelocity;
            horizontalVelocity.y = 0f;

            bool hasInput = moveInput.sqrMagnitude > 0.001f;

            // Brake on the ground; preserve airborne momentum without input.
            if (grounded || hasInput)
            {
                float acceleration = grounded
                    ? (hasInput ? groundAcceleration : groundBraking) : airAcceleration;

                Vector3 nextVelocity = Vector3.MoveTowards(horizontalVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);

                // Adjust horizontal motion without changing jump/fall velocity.
                rb.AddForce(nextVelocity - horizontalVelocity, ForceMode.VelocityChange);
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
    public void ChangeOxygen(float amount)//void for changing oxygen level without going above max or below 0.
    {
        oxygen = Mathf.Clamp(oxygen + amount, 0f, maxOxygen);
    }
    #endregion
}
