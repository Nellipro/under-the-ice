using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class SubmarineMovement : MonoBehaviour
{

    public Transform thisShip;
    public Rigidbody rb;
    public SwitchPullSystem throttleLever;
    public SwitchRotateSystem yawLever;
    public SwitchRotateSystem pitchLever;

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
    private float thrustValue;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }


    void FixedUpdate()
    {
        turn();
        thrust();
    }

    void turn()
    {
        float Y = yawLever.value;
        float P = pitchLever.value;

        yaw = (turnSpeed * Y);
        pitch = (turnSpeed * P);
        //roll = turnSpeed * Time.deltaTime * Input.GetAxis("Rotate");
        thisShip.Rotate(pitch, yaw, roll);
    }

    void thrust()
    {
        thrustValue = throttleLever.value;
        rb.AddForce(thisShip.forward * thrustSpeed * (thrustValue * 100));
    }
}
