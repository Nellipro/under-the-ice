using UnityEngine;

/// <summary>
/// Transform-driven submarine movement for the mesh-collider submarine.
/// The legacy Rigidbody-based controller remains available separately.
/// </summary>
public class SubmarineMovementTransform : MonoBehaviour
{
    [Header("Ship")]
    public Transform thisShip;

    [Header("Controls")]
    public SwitchPullSystem forwardLever;
    public SwitchPullSystem backwardLever;
    public SwitchRotateSystem leftLever;
    public SwitchRotateSystem rightLever;
    public SwitchRotateSystem upLever;
    public SwitchRotateSystem downLever;

    [Header("Movement Settings")]
    [Tooltip("Turn rate in degrees per second at full lever input.")]
    public float turnSpeed = 50f;

    [Tooltip("Acceleration in world units per second squared at full thrust.")]
    public float thrustSpeed = 45f;

    [Tooltip("How quickly the submarine slows when thrust is released, in inverse seconds.")]
    public float movementDamping = 10f;

    public bool isActive;

    private Vector3 velocity;

    private void Awake()
    {
        if (thisShip == null)
        {
            thisShip = transform;
        }

        // Match the legacy controller: the engine starts off until switched on.
        isActive = false;
    }

    private void FixedUpdate()
    {
        if (thisShip == null)
        {
            return;
        }

        float deltaTime = Time.fixedDeltaTime;

        if (isActive)
        {
            ApplySteering(deltaTime);
            ApplyThrust(deltaTime);
        }

        // The old Rigidbody supplied this coasting behavior through linear damping.
        // Apply it explicitly now that motion is driven by the Transform.
        velocity *= Mathf.Exp(-Mathf.Max(0f, movementDamping) * deltaTime);
        thisShip.position += velocity * deltaTime;
    }

    private void ApplySteering(float deltaTime)
    {
        float yawInput = GetValue(rightLever) - GetValue(leftLever);
        float yawDelta = yawInput * turnSpeed * deltaTime;

        thisShip.Rotate(0f, yawDelta, 0f, Space.Self);
    }

    private void ApplyThrust(float deltaTime)
    {
        float forwardInput = GetValue(forwardLever) - GetValue(backwardLever);
        float verticalInput = GetValue(upLever) - GetValue(downLever);

        Vector3 acceleration = thisShip.forward * forwardInput
                             + thisShip.up * verticalInput;

        velocity += acceleration * thrustSpeed * deltaTime;
    }

    private static float GetValue(SwitchPullSystem lever)
    {
        return lever != null ? lever.value : 0f;
    }

    private static float GetValue(SwitchRotateSystem lever)
    {
        return lever != null ? lever.value : 0f;
    }
}
