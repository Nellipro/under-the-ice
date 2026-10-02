using UnityEngine;

public class SwitchRotateSystem : MonoBehaviour, IInteractible
{
    Transform leverHandle;
    public Vector3 rotationAxis = Vector3.right;
    public float upAngle = -35f;
    public float downAngle = 35f;
    public float moveSpeed = 8f;
    public float rayAngleOffset;
    public float lockValue;

    private Quaternion startRotation;
    private float targetAngle;
    private float currentAngle;
    private bool isBeingHeld;
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
        currentAngle = upAngle;
        value = 0f;
        isBeingHeld = false;
    }

    void Update()
    {
        currentAngle = Mathf.Lerp(currentAngle, targetAngle, moveSpeed * Time.deltaTime);
        Quaternion targetRotation = startRotation * Quaternion.AngleAxis(targetAngle, rotationAxis.normalized);
        leverHandle.localRotation = Quaternion.Slerp(leverHandle.localRotation, targetRotation, moveSpeed * Time.deltaTime);
        value = Mathf.InverseLerp(upAngle, downAngle, currentAngle);

        if (value <= lockValue & !isBeingHeld)
        {
            value = Mathf.Clamp01(0);

            currentAngle = Mathf.Lerp(upAngle, downAngle, value);
            targetAngle = currentAngle;
            targetRotation = startRotation * Quaternion.AngleAxis(targetAngle, rotationAxis.normalized);

            leverHandle.localRotation = targetRotation;
            isDown = value >= 0.5f;
        }
    }

    public void Interact()
    {
        isDown = !isDown;
        targetAngle = isDown ? downAngle : upAngle;
        isBeingHeld = true;
    }

    public void TurnOff()
    {
        isDown = false;
        targetAngle = upAngle;
    }

    public void Pull(Vector3 rayHitPoint)
    {
        Vector3 localHitPoint = leverHandle.InverseTransformPoint(rayHitPoint);
        Vector3 localDirection = Vector3.ProjectOnPlane(localHitPoint, rotationAxis.normalized);

        if (localDirection.sqrMagnitude > 0.001f)
        {
            targetAngle = Mathf.Clamp(
                Vector3.SignedAngle(Vector3.forward, localDirection, rotationAxis.normalized) + rayAngleOffset,
                Mathf.Min(upAngle, downAngle),
                Mathf.Max(upAngle, downAngle));
        }
    }

    public void Release()
    {
        isBeingHeld = false;
    }
}
