using UnityEngine;

public class SubmarineMovement : MonoBehaviour
{

    public Transform thisShip;
    public Rigidbody rb;
    public SwitchPullSystem forwardLever;
    public SwitchPullSystem backwardLever;
    public SwitchRotateSystem steeringLever;
    public SwitchRotateSystem upLever;
    public SwitchRotateSystem downLever;

    [Header("Movement Settings")]
    public  float turnSpeed = 60f;
    public float thrustSpeed = 45f;
    public bool isActive;
    private float yaw;
    private float pitch;
    private float roll;
    private float forwardValue;
    private float backwardValue;
    private float upValue;
    private float downValue;
    public float stabilizationForce;

    [Header("sub carry system")]
    public Rigidbody submarineRb;
    public Rigidbody playerRb;
    private Vector3 lastPosition;
    private Quaternion lastRotation;

    private void Awake()
    {
        lastPosition = submarineRb.position;
        lastRotation = submarineRb.rotation;
    }

    void Start()
    {
        isActive = false;
        rb = GetComponent<Rigidbody>();
    }


    void FixedUpdate()
    {
        if(isActive)
        {
            Turn();
            Thrust();
            GoUpAndDown();
        }
    }

    void Turn()
    {
        yaw = turnSpeed * steeringLever.value;
        thisShip.Rotate(pitch, yaw, roll);
    }

    void Thrust()
    {
        forwardValue = forwardLever.value;
        rb.AddForce(thisShip.forward * thrustSpeed * (forwardValue * 100));

        backwardValue = backwardLever.value;
        rb.AddForce(-(thisShip.forward * thrustSpeed * (backwardValue * 100)));
    }

    void GoUpAndDown()
    {
        upValue = upLever.value;
        rb.AddForce(thisShip.up * thrustSpeed * (upValue * 100));

        downValue = downLever.value;
        rb.AddForce(-(thisShip.up * thrustSpeed * (downValue * 100)));
    }
}
