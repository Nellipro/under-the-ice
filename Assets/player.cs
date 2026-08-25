using UnityEngine;
using UnityEngine.InputSystem;
public class player : MonoBehaviour
{
    [SerializeField] private bool isInWater = false;
    [SerializeField] private float speed = 5.0f;
    public Vector2 moveInput;
    public Vector2 viewInput;
    [SerializeField] private GameObject camera;
    private bool freezePlayer = false;  
    [SerializeField] private playerUI playerUI; // Reference to the playerUI script
    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }
    void OnLook(InputValue value)
    {
        viewInput = value.Get<Vector2>();
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
        


    }
    void FixedUpdate()
    {
    
        Rigidbody rb = GetComponent<Rigidbody>();
        if (isInWater)
        {
            // Apply water physics
            // For example, reduce gravity
                rb.AddForce(Vector3.down * 2.0f); // Reduced gravity force in water

        }
       
       
        // normal submarine physics for the player
        if (!isInWater)
        {
            // Apply normal physics
            rb.AddForce(Vector3.down * 9.81f); // Normal gravity force
            
            Vector3 moveDirection = transform.right * moveInput.x + transform.forward * moveInput.y;
            
            rb.AddForce(moveDirection.normalized * speed, ForceMode.Force);

        }
        if (camera != null)
        {
            // Rotate the camera based on view input
            camera.transform.Rotate(-viewInput.y, viewInput.x, 0);
        }


    }
}
