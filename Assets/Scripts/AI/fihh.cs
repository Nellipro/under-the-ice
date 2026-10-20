using UnityEngine;
using System.Collections.Generic;

public class fihh : MonoBehaviour
{

     [Header("Pathfinding")]
    [SerializeField] private WaypointNetwork3D waypointNetwork;
    [SerializeField, Min(1f)] private float minTravelTime = 30f;
    [SerializeField, Min(1f)] private float maxTravelTime = 120f;
    [SerializeField, Min(0.05f)] private float waypointTolerance = 1f;

    [Header("Movement")]
    [SerializeField, Min(0.1f)] private float swimSpeed = 3f;
    [SerializeField, Min(0.1f)] private float acceleration = 5f;
    [SerializeField, Min(0.1f)] private float turnSpeed = 3f;
    [SerializeField] private Collider arrivalCollider;

    [Header("Debug")]
    [SerializeField] private bool drawPath = true;

    private Rigidbody rb;
    private Renderer[] fishRenderers;
    private readonly List<Vector3> currentPath = new();

    private int pathIndex;
    private int airTriggerCount;
    private float nextDestinationTime;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;

        if (arrivalCollider == null)
        {
            arrivalCollider = GetComponentInChildren<Collider>();
        }

        // This finds both MeshRenderers and SkinnedMeshRenderers.
        fishRenderers = GetComponentsInChildren<Renderer>(true);
    }

    private void Start()
    {
        PickNewDestination();
    }

    private void Update()
    {
        // This timer starts as soon as a destination is picked.
        // The fish does NOT need to reach its target before it picks another one.
        if (Time.time >= nextDestinationTime)
        {
            PickNewDestination();
        }
    }

    private void FixedUpdate()
    {
        if (currentPath.Count == 0 || pathIndex >= currentPath.Count)
        {
            SlowDown();
            return;
        }

        // Skip every path point the fish has already reached.
        while (pathIndex < currentPath.Count &&
               HasReachedWaypoint(currentPath[pathIndex]))
        {
            pathIndex++;
        }

        if (pathIndex >= currentPath.Count)
        {
            SlowDown();
            return;
        }

        Vector3 directionToWaypoint =
            currentPath[pathIndex] - rb.position;

        Vector3 direction = directionToWaypoint.normalized;
        Vector3 desiredVelocity = direction * swimSpeed;

        rb.linearVelocity = Vector3.MoveTowards(
            rb.linearVelocity,
            desiredVelocity,
            acceleration * Time.fixedDeltaTime);

        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion wantedRotation =
                Quaternion.LookRotation(direction, Vector3.up);

            rb.MoveRotation(Quaternion.Slerp(
                rb.rotation,
                wantedRotation,
                turnSpeed * Time.fixedDeltaTime));
        }
    }

    private void PickNewDestination()
    {
        // Pick the next reroute time immediately.
        float travelTime = Random.Range(minTravelTime, maxTravelTime);
        nextDestinationTime = Time.time + travelTime;

        if (waypointNetwork == null)
        {
            Debug.LogWarning(
                $"{name} has no Waypoint Network assigned.",
                this);

            currentPath.Clear();
            return;
        }

        Vector3 destination =
            waypointNetwork.GetRandomWaypointPosition(transform.position);

        List<Vector3> newPath =
            waypointNetwork.FindPatrolPath(transform.position, destination);

        currentPath.Clear();
        currentPath.AddRange(newPath);
        pathIndex = 0;
    }

    private bool HasReachedWaypoint(Vector3 waypoint)
    {
        // Fallback if no collider was assigned or found.
        if (arrivalCollider == null || !arrivalCollider.enabled)
        {
            return Vector3.Distance(rb.position, waypoint)
                   <= waypointTolerance;
        }

        // If the waypoint is inside the collider, ClosestPoint returns
        // the waypoint itself. Tolerance expands this arrival area slightly.
        Vector3 closestPoint = arrivalCollider.ClosestPoint(waypoint);

        return (closestPoint - waypoint).sqrMagnitude
               <= waypointTolerance * waypointTolerance;
    }

    private void SlowDown()
    {
        rb.linearVelocity = Vector3.MoveTowards(
            rb.linearVelocity,
            Vector3.zero,
            acceleration * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Air"))
        {
            return;
        }

        airTriggerCount++;
        SetFishVisible(false);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Air"))
        {
            return;
        }

        airTriggerCount = Mathf.Max(0, airTriggerCount - 1);

        if (airTriggerCount == 0)
        {
            SetFishVisible(true);
        }
    }

    private void SetFishVisible(bool isVisible)
    {
        foreach (Renderer fishRenderer in fishRenderers)
        {
            fishRenderer.enabled = isVisible;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!drawPath || currentPath.Count == 0)
        {
            return;
        }

        Gizmos.color = Color.green;

        Vector3 previousPoint = transform.position;

        for (int index = pathIndex; index < currentPath.Count; index++)
        {
            Gizmos.DrawLine(previousPoint, currentPath[index]);
            Gizmos.DrawSphere(currentPath[index], 0.15f);
            previousPoint = currentPath[index];
        }
    }
}
