using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class SubmarineMovement : MonoBehaviour
{

    public Transform thisShip;
    public Rigidbody rb;
    public SwitchPullSystem forwardLever;
    public SwitchPullSystem backwardLever;
    public SwitchRotateSystem leftLever;
    public SwitchRotateSystem rightLever;
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

            if (Mathf.Abs(upLever.value) + Mathf.Abs(downLever.value) < 0.1f)
            {
                if (Mathf.Abs(leftLever.value) + Mathf.Abs(rightLever.value) < 0.1f)
                {
                    Stabilize();
                }               
            }
        }
    }

    void Turn()
    {
        float Y = leftLever.value + -rightLever.value;
        yaw = (turnSpeed * Y);
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

    void Stabilize()
    {
        Vector3 currentRotation = thisShip.eulerAngles;

        if (currentRotation.x > 180) currentRotation.x -= 360;
        if (currentRotation.z > 180) currentRotation.z -= 360;

            rb.AddTorque(-currentRotation.x * stabilizationForce, 0, 0);       
            rb.AddTorque(0, 0, -currentRotation.z * stabilizationForce);
    }
}
