using UnityEngine;

public class SubmarineMovement : MonoBehaviour
{

    public Transform thisShip;
    public Rigidbody rb;

    [Header("Movement Settings")]
    public  float turnSpeed = 60f;
    public float thrustSpeed = 45f;
    public float boostSpeed = 100f;
    public float boostFuel = 100f;
    public float maxBoostFuel = 100f;
    public float linDaming = 1f;
    public float angDamping = 1f;
    private float yaw;
    private float pitch;
    private float roll;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }


    void Update()
    {
        turn();
        thrust();
    }

    void turn()
    {
        yaw = turnSpeed * Time.deltaTime * Input.GetAxis("Horizontal");
        pitch = turnSpeed * Time.deltaTime * Input.GetAxis("Vertical");
        roll = turnSpeed * Time.deltaTime * Input.GetAxis("Rotate");
        thisShip.Rotate(pitch, yaw, roll);
    }

    void thrust()
    {
        rb.AddForce(thisShip.forward * thrustSpeed * Input.GetAxis("Throttle"));
    }
}
