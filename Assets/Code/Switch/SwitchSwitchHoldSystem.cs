using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class SwitchSwitchHoldSystem : MonoBehaviour, IInteractible
{
    Transform leverHandle;
    public Vector3 rotationAxis = Vector3.right;
    public float upAngle = -35f;
    public float downAngle = 35f;
    public float moveSpeed = 8f;

    private Quaternion startRotation;
    private float targetAngle;
    private bool isDown;

    public float value = 0;

    [Header("the switchObject")]
    public SwitchObject switchObject;
/////////////////////////////////////////

    public float Value => value;

    void Awake()
    {
        if (leverHandle == null)
        {
            leverHandle = transform;
        }

        startRotation = leverHandle.localRotation;
        targetAngle = upAngle;
        value = 0;
    }

    void Update()
    {
        Quaternion targetRotation = startRotation * Quaternion.AngleAxis(targetAngle, rotationAxis.normalized);
        leverHandle.localRotation = Quaternion.Slerp(leverHandle.localRotation, targetRotation, moveSpeed * Time.deltaTime);

        if (isDown)
        {
            value = 1;
        }
        else if (!isDown)
        {
            value = 0;
        }
    }

    public void Interact()
    {
        targetAngle = downAngle;
        isDown = true;
    }

    public void TurnOff()
    {
        targetAngle = upAngle;
        isDown = false;
    }

    public void Pull(Vector3 rayHitPoint)
    {
        targetAngle = downAngle;
        isDown = true;
    }

    public void Release()
    {
        targetAngle = upAngle;
        isDown = false;
    }
}
