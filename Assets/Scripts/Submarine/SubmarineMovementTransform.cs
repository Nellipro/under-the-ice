using UnityEngine;

/// <summary>
/// Moves the mesh-collider submarine without relying on non-kinematic Rigidbody
/// forces. The legacy Rigidbody controller remains available separately.
/// </summary>
public class SubmarineMovementTransform : MonoBehaviour
{
    [Header("Ship")]
    public Transform thisShip;

    [Header("Controls")]
    public SwitchPullSystem forwardLever;
    public SwitchPullSystem backwardLever;
    public SwitchRotateSystem steeringLever;
    public SwitchRotateSystem upLever;
    public SwitchRotateSystem downLever;

    [Header("Movement Settings")]
    [Tooltip("Turn rate in degrees per second at full lever input.")]
    public float turnSpeed = 50f;

    [Tooltip("Acceleration in world units per second squared at full thrust.")]
    public float thrustSpeed = 45f;

    [Tooltip("How quickly the submarine slows when thrust is released, in inverse seconds.")]
    public float movementDamping = 10f;

    [Tooltip("Extra clearance used by the hull sweep when checking nearby geometry.")]
    public float collisionPadding = 0.05f;

    [Tooltip("Small gap to leave between the submarine and a wall when stopping.")]
    public float collisionSkin = 0.08f;

    public bool isActive;

    private Vector3 velocity;
    private Rigidbody shipRigidbody;
    private SubmarineMovement legacyMovement;
    private Bounds localHullBounds;
    private bool hasHullBounds;

    private void Awake()
    {
        if (thisShip == null)
        {
            thisShip = transform;
        }

        legacyMovement = GetComponent<SubmarineMovement>();
        if (legacyMovement != null)
        {
            // The existing switch may still toggle the legacy component's flag.
            // Keep its FixedUpdate from fighting this controller and read that flag below.
            if (steeringLever == null) steeringLever = legacyMovement.steeringLever;
            if (forwardLever == null) forwardLever = legacyMovement.forwardLever;
            if (backwardLever == null) backwardLever = legacyMovement.backwardLever;
            if (upLever == null) upLever = legacyMovement.upLever;
            if (downLever == null) downLever = legacyMovement.downLever;
            legacyMovement.enabled = false;
        }

        if (thisShip != null)
        {
            shipRigidbody = thisShip.GetComponent<Rigidbody>();
            if (shipRigidbody == null)
            {
                shipRigidbody = thisShip.gameObject.AddComponent<Rigidbody>();
            }

            // A kinematic body supports the submarine's non-convex mesh colliders.
            // Swept queries below provide the movement blocking that kinematic bodies
            // do not get from forces or collision response.
            shipRigidbody.isKinematic = true;
            shipRigidbody.useGravity = false;
            shipRigidbody.interpolation = RigidbodyInterpolation.Interpolate;

            CacheLocalHullBounds();
        }

        // Match the legacy controller: the engines start off until switched on.
        isActive = false;
    }

    private void FixedUpdate()
    {
        if (thisShip == null || shipRigidbody == null)
        {
            return;
        }

        float deltaTime = Time.fixedDeltaTime;
        Vector3 oldPosition = thisShip.position;
        Quaternion oldRotation = thisShip.rotation;
        Quaternion targetRotation = oldRotation;

        if (EngineIsActive())
        {
            float yawDelta = GetValue(steeringLever) * turnSpeed * deltaTime;
            if (Mathf.Abs(yawDelta) > Mathf.Epsilon)
            {
                Quaternion proposedRotation = oldRotation * Quaternion.Euler(0f, yawDelta, 0f);
                if (IsPoseClear(oldPosition, proposedRotation))
                {
                    targetRotation = proposedRotation;
                }
            }

            ApplyThrust(deltaTime, targetRotation);
        }

        // Preserve the coasting behavior of the old Rigidbody-driven controller.
        velocity *= Mathf.Exp(-Mathf.Max(0f, movementDamping) * deltaTime);

        Vector3 targetPosition = SweepTranslation(oldPosition, targetRotation, velocity * deltaTime);

        // Move the kinematic hull through the physics step instead of teleporting its Transform.
        shipRigidbody.MovePosition(targetPosition);
        shipRigidbody.MoveRotation(targetRotation);

    }

    private bool EngineIsActive()
    {
        return isActive || (legacyMovement != null && legacyMovement.isActive);
    }

    private void ApplyThrust(float deltaTime, Quaternion shipRotation)
    {
        float forwardInput = GetValue(forwardLever) - GetValue(backwardLever);
        float verticalInput = GetValue(upLever) - GetValue(downLever);

        Vector3 acceleration = shipRotation * Vector3.forward * forwardInput
                             + shipRotation * Vector3.up * verticalInput;

        velocity += acceleration * thrustSpeed * deltaTime;
    }

    private Vector3 SweepTranslation(Vector3 startPosition, Quaternion orientation, Vector3 displacement)
    {
        float distance = displacement.magnitude;
        if (distance <= Mathf.Epsilon)
        {
            return startPosition;
        }

        Vector3 direction = displacement / distance;
        GetWorldHullBox(startPosition, orientation, out Vector3 center, out Vector3 halfExtents);

        RaycastHit[] hits = Physics.BoxCastAll(
            center,
            halfExtents,
            direction,
            orientation,
            distance,
            Physics.AllLayers,
            QueryTriggerInteraction.Ignore);

        float allowedDistance = distance;
        RaycastHit blockingHit = default;
        bool foundBlockingHit = false;

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider == null || ShouldIgnoreCollider(hit.collider))
            {
                continue;
            }

            if (!foundBlockingHit || hit.distance < blockingHit.distance)
            {
                blockingHit = hit;
                foundBlockingHit = true;
            }
        }

        if (foundBlockingHit)
        {
            allowedDistance = Mathf.Clamp(blockingHit.distance - Mathf.Max(0f, collisionSkin), 0f, distance);

            // Remove only the velocity component pushing into the obstacle so the ship
            // can continue sliding along it instead of sticking or tunneling through.
            float velocityIntoSurface = Vector3.Dot(velocity, blockingHit.normal);
            if (velocityIntoSurface < 0f)
            {
                velocity -= blockingHit.normal * velocityIntoSurface;
            }
        }

        return startPosition + direction * allowedDistance;
    }

    private bool IsPoseClear(Vector3 position, Quaternion orientation)
    {
        GetWorldHullBox(position, orientation, out Vector3 center, out Vector3 halfExtents);

        Collider[] overlaps = Physics.OverlapBox(
            center,
            halfExtents,
            orientation,
            Physics.AllLayers,
            QueryTriggerInteraction.Ignore);

        foreach (Collider overlap in overlaps)
        {
            if (overlap != null && !ShouldIgnoreCollider(overlap))
            {
                return false;
            }
        }

        return true;
    }

    private bool ShouldIgnoreCollider(Collider collider)
    {
        Transform colliderTransform = collider.transform;
        if (colliderTransform == thisShip || colliderTransform.IsChildOf(thisShip))
        {
            return true;
        }

        // The legacy interior/hull Rigidbody can overlap the new exterior hull in the scene.
        if (legacyMovement != null && legacyMovement.rb != null
            && collider.attachedRigidbody == legacyMovement.rb)
        {
            return true;
        }

        return false;
    }

    private void GetWorldHullBox(
        Vector3 position,
        Quaternion orientation,
        out Vector3 center,
        out Vector3 halfExtents)
    {
        Vector3 scale = thisShip.lossyScale;
        scale = new Vector3(Mathf.Abs(scale.x), Mathf.Abs(scale.y), Mathf.Abs(scale.z));

        Vector3 localCenter = hasHullBounds ? localHullBounds.center : Vector3.zero;
        Vector3 localExtents = hasHullBounds ? localHullBounds.extents : Vector3.one * 0.5f;

        center = position + orientation * Vector3.Scale(localCenter, scale);
        halfExtents = Vector3.Scale(localExtents, scale) + Vector3.one * Mathf.Max(0f, collisionPadding);
    }

    private void CacheLocalHullBounds()
    {
        Collider[] colliders = thisShip.GetComponentsInChildren<Collider>(true);
        bool foundBounds = false;
        Bounds bounds = default;

        foreach (Collider hullCollider in colliders)
        {
            if (hullCollider == null || !hullCollider.enabled || hullCollider.isTrigger)
            {
                continue;
            }

            Bounds worldBounds = hullCollider.bounds;
            for (int cornerIndex = 0; cornerIndex < 8; cornerIndex++)
            {
                Vector3 corner = worldBounds.center + Vector3.Scale(
                    worldBounds.extents,
                    new Vector3(
                        (cornerIndex & 1) == 0 ? -1f : 1f,
                        (cornerIndex & 2) == 0 ? -1f : 1f,
                        (cornerIndex & 4) == 0 ? -1f : 1f));
                Vector3 localCorner = thisShip.InverseTransformPoint(corner);

                if (!foundBounds)
                {
                    bounds = new Bounds(localCorner, Vector3.zero);
                    foundBounds = true;
                }
                else
                {
                    bounds.Encapsulate(localCorner);
                }
            }
        }

        hasHullBounds = foundBounds;
        localHullBounds = foundBounds ? bounds : new Bounds(Vector3.zero, Vector3.one);
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
