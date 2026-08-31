using UnityEngine;
using UnityEngine.InputSystem;
public class player : MonoBehaviour
{
    [SerializeField] bool isInWater = false;
    [SerializeField] private float speed = 5.0f;
    public Vector2 moveInput;
    public Vector2 viewInput;
    private float camPitch;
    [SerializeField] private GameObject playerCamera;
    private bool freezePlayer = false;  
    [SerializeField] private playerUI playerUI; // Reference to the playerUI script
    [SerializeField] private float oxygen = 100f;
    [SerializeField] private int maxOxygen = 100; // Maximum oxygen level
    bool freezeCam = false;

    public float camSens;

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }
    void OnLook(InputValue value)
    {
        viewInput = value.Get<Vector2>();
        viewInput *= camSens; // Adjust sensitivity as needed
    }
    //void for changing oxygen level without going above max or below 0.
    void ChangeOxygen(float amount)
    {
        oxygen = Mathf.Clamp(oxygen + amount, 0f, maxOxygen);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        freezePlayer = playerUI.ShowUI; // Update freezePlayer based on ShowUI from playerUI
        if (freezePlayer)
        {
            
        }
        freezeCam = playerUI.ShowUI;


    }
    void FixedUpdate()
    {
    
        Rigidbody rb = GetComponent<Rigidbody>();
        if (isInWater)
        {
            // Apply water physics
            // For example, reduce gravity
                rb.AddForce(Vector3.down * 2.0f); // Reduced gravity force in water 
                // idea ... new Vector3(Camera.transform.forward.x, 0f, Camera.transform.forward.z).normalized;
        }
       
       
        // normal submarine physics for the player
        if (!isInWater)
        {
            // Apply normal physics
            rb.AddForce(Vector3.down * 9.81f); // Normal gravity force
            
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


    }
}