using UnityEngine;

public class TempBox : MonoBehaviour, IInteractible
{
    public Transform leverHandle;
    public Vector3 rotationAxis = Vector3.right;
    public float upAngle = -35f;
    public float downAngle = 35f;
    public float moveSpeed = 8f;
    public float rayAngleOffset;

    private Quaternion startRotation;
    private float targetAngle;
    private bool isDown;

    void Awake()
    {
        if (leverHandle == null)
        {
            leverHandle = transform;
        }

        startRotation = leverHandle.localRotation;
        targetAngle = upAngle;
    }

    void Update()
    {
        Quaternion targetRotation = startRotation * Quaternion.AngleAxis(targetAngle, rotationAxis.normalized);
        leverHandle.localRotation = Quaternion.Slerp(leverHandle.localRotation, targetRotation, moveSpeed * Time.deltaTime);
    }

    public void Interact()
    {
        isDown = !isDown;
        targetAngle = isDown ? downAngle : upAngle;
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
    }
}
