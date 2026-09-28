using UnityEngine;

/// <summary>
/// A point in the volume that underwater enemies are allowed to swim through.
/// Put waypoint objects underneath a WaypointNetwork3D object.
/// </summary>
public sealed class UnderwaterWaypoint : MonoBehaviour
{
    private const float GizmoRadius = 0.25f;

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.1f, 0.8f, 1f, 0.85f);
        Gizmos.DrawSphere(transform.position, GizmoRadius);
    }
}
