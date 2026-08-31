using UnityEngine;

public class SwitchSwitchSystem : MonoBehaviour, IInteractible
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
            if (switchObject != null)
            {
                switchObject.ToggleObject(true);
            }    
        }
        else if (!isDown)
        {
            value = 0;
            if (switchObject != null)
            {
                switchObject.ToggleObject(false);
            }
        }
    }

    public void Interact()
    {
        isDown = !isDown;
        targetAngle = isDown ? downAngle : upAngle;
    }

    public void TurnOff()
    {
        isDown = false;
        targetAngle = upAngle;
    }

    public void Pull(Vector3 rayHitPoint)
    {
    }

    public void Release()
    {
    }


}
