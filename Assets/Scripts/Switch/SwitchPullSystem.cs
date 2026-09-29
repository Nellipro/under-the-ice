using UnityEngine;

public class SwitchPullSystem : MonoBehaviour, IInteractible
{
    Transform leverHandle;
    public Vector3 movementAxis = Vector3.up;
    public float upDistance = -0.35f;
    public float downDistance = 0.35f;
    public float moveSpeed = 8f;

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float currentDistance;
    private float targetDistance;
    private Vector3 normalizedMovementAxis;
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

        startPosition = leverHandle.localPosition;
        normalizedMovementAxis = movementAxis.sqrMagnitude > 0.001f
            ? movementAxis.normalized
            : Vector3.up;
        currentDistance = upDistance;
        targetDistance = upDistance;
        targetPosition = startPosition + normalizedMovementAxis * currentDistance;
        value = 0f;
    }

    void Update()
    {
        currentDistance = Mathf.Lerp(currentDistance, targetDistance, moveSpeed * Time.deltaTime);
        targetPosition = startPosition + normalizedMovementAxis * currentDistance;
        leverHandle.localPosition = Vector3.Lerp(leverHandle.localPosition, targetPosition, moveSpeed * Time.deltaTime);
        value = Mathf.InverseLerp(upDistance, downDistance, currentDistance);
    }

    public void Interact()
    {
        isDown = !isDown;
        targetDistance = isDown ? downDistance : upDistance;
    }

    public void TurnOff()
    {
        isDown = false;
        targetDistance = upDistance;
    }

    public void Pull(Vector3 rayHitPoint)
    {
        Vector3 parentSpaceHitPoint = leverHandle.parent == null
            ? rayHitPoint
            : leverHandle.parent.InverseTransformPoint(rayHitPoint);
        float hitDistance = Vector3.Dot(parentSpaceHitPoint - startPosition, normalizedMovementAxis);
        float minimumDistance = Mathf.Min(upDistance, downDistance);
        float maximumDistance = Mathf.Max(upDistance, downDistance);

        if (Mathf.Abs(maximumDistance - minimumDistance) > 0.001f)
        {
            SetTargetDistance(Mathf.Clamp(hitDistance, minimumDistance, maximumDistance));
        }
    }

    public void Release()
    {
    }

    private void SetTargetDistance(float distance)
    {
        targetDistance = distance;
        isDown = Mathf.Abs(distance - downDistance) < Mathf.Abs(distance - upDistance);
    }

}
