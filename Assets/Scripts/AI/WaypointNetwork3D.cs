using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Builds a simple 3D waypoint graph and uses A* to find routes through it.
/// This is intended for a small number of underwater enemies and waypoints.
/// </summary>
public sealed class WaypointNetwork3D : MonoBehaviour
{
    [Header("Connections")]
    [SerializeField, Min(0.1f)] private float maximumConnectionDistance = 15f;
    [SerializeField, Min(0f)] private float creatureClearanceRadius = 0.75f;
    [SerializeField] private LayerMask obstacleLayers = 1 << 10;

    [Header("Debug")]
    [SerializeField] private bool drawConnections = true;
    [SerializeField] private bool drawConnectionsAlways;

    private readonly Dictionary<UnderwaterWaypoint, List<UnderwaterWaypoint>> connections = new();
    private UnderwaterWaypoint[] waypoints = System.Array.Empty<UnderwaterWaypoint>();

    public LayerMask ObstacleLayers => obstacleLayers;
    public bool HasWaypoints => waypoints.Length > 0;

    private void Awake()
    {
        RebuildNetwork();
    }

    [ContextMenu("Rebuild Network")]
    public void RebuildNetwork()
    {
        waypoints = GetComponentsInChildren<UnderwaterWaypoint>(true);
        connections.Clear();

        foreach (UnderwaterWaypoint waypoint in waypoints)
        {
            connections[waypoint] = new List<UnderwaterWaypoint>();
        }

        for (int first = 0; first < waypoints.Length; first++)
        {
            for (int second = first + 1; second < waypoints.Length; second++)
            {
                UnderwaterWaypoint a = waypoints[first];
                UnderwaterWaypoint b = waypoints[second];

                if (Vector3.Distance(a.transform.position, b.transform.position) > maximumConnectionDistance)
                {
                    continue;
                }

                if (!HasClearPath(a.transform.position, b.transform.position))
                {
                    continue;
                }

                connections[a].Add(b);
                connections[b].Add(a);
            }
        }
    }

    /// <summary>
    /// Returns world positions from start to target. An empty list means no route was found.
    /// </summary>
    public List<Vector3> FindPath(Vector3 start, Vector3 target)
    {
        if (HasClearPath(start, target))
        {
            return new List<Vector3> { target };
        }

        UnderwaterWaypoint startNode = FindClosestVisibleWaypoint(start);
        UnderwaterWaypoint targetNode = FindClosestVisibleWaypoint(target);

        if (startNode == null || targetNode == null)
        {
            return new List<Vector3>();
        }

        List<UnderwaterWaypoint> nodePath = FindNodePath(startNode, targetNode);
        if (nodePath.Count == 0)
        {
            return new List<Vector3>();
        }

        var result = new List<Vector3>(nodePath.Count + 1);
        foreach (UnderwaterWaypoint node in nodePath)
        {
            result.Add(node.transform.position);
        }

        result.Add(target);
        SmoothPath(start, result);
        return result;
    }

    /// <summary>
    /// Chooses a waypoint away from a threat and returns a path to it.
    /// </summary>
    public List<Vector3> FindFleePath(Vector3 start, Vector3 threatPosition)
    {
        if (waypoints.Length == 0)
        {
            return new List<Vector3>();
        }

        var candidates = new List<UnderwaterWaypoint>(waypoints);
        candidates.Sort((a, b) =>
        {
            float scoreA = FleeScore(a.transform.position, start, threatPosition);
            float scoreB = FleeScore(b.transform.position, start, threatPosition);
            return scoreB.CompareTo(scoreA);
        });

        // Try several candidates in case the farthest waypoint is in another graph island.
        int attempts = Mathf.Min(candidates.Count, 8);
        for (int index = 0; index < attempts; index++)
        {
            List<Vector3> path = FindPath(start, candidates[index].transform.position);
            if (path.Count > 0)
            {
                return path;
            }
        }

        return new List<Vector3>();
    }

    public Vector3 GetRandomWaypointPosition(Vector3 fallback)
    {
        return waypoints.Length == 0
            ? fallback
            : waypoints[Random.Range(0, waypoints.Length)].transform.position;
    }

    public bool HasClearPath(Vector3 start, Vector3 target)
    {
        Vector3 offset = target - start;
        float distance = offset.magnitude;

        if (distance <= 0.01f)
        {
            return true;
        }

        return !Physics.SphereCast(
            start,
            creatureClearanceRadius,
            offset / distance,
            out _,
            distance,
            obstacleLayers,
            QueryTriggerInteraction.Ignore);
    }

    private UnderwaterWaypoint FindClosestVisibleWaypoint(Vector3 position)
    {
        UnderwaterWaypoint closest = null;
        float closestSquaredDistance = float.PositiveInfinity;

        foreach (UnderwaterWaypoint waypoint in waypoints)
        {
            float squaredDistance = (waypoint.transform.position - position).sqrMagnitude;
            if (squaredDistance >= closestSquaredDistance || !HasClearPath(position, waypoint.transform.position))
            {
                continue;
            }

            closest = waypoint;
            closestSquaredDistance = squaredDistance;
        }

        return closest;
    }

    private List<UnderwaterWaypoint> FindNodePath(UnderwaterWaypoint start, UnderwaterWaypoint target)
    {
        var open = new List<UnderwaterWaypoint> { start };
        var closed = new HashSet<UnderwaterWaypoint>();
        var cameFrom = new Dictionary<UnderwaterWaypoint, UnderwaterWaypoint>();
        var costFromStart = new Dictionary<UnderwaterWaypoint, float> { [start] = 0f };

        while (open.Count > 0)
        {
            UnderwaterWaypoint current = GetLowestCostNode(open, costFromStart, target);
            if (current == target)
            {
                return ReconstructPath(cameFrom, current);
            }

            open.Remove(current);
            closed.Add(current);

            foreach (UnderwaterWaypoint neighbour in connections[current])
            {
                if (closed.Contains(neighbour))
                {
                    continue;
                }

                float proposedCost = costFromStart[current] +
                    Vector3.Distance(current.transform.position, neighbour.transform.position);

                if (costFromStart.TryGetValue(neighbour, out float knownCost) && proposedCost >= knownCost)
                {
                    continue;
                }

                cameFrom[neighbour] = current;
                costFromStart[neighbour] = proposedCost;

                if (!open.Contains(neighbour))
                {
                    open.Add(neighbour);
                }
            }
        }

        return new List<UnderwaterWaypoint>();
    }

    private static UnderwaterWaypoint GetLowestCostNode(
        List<UnderwaterWaypoint> open,
        Dictionary<UnderwaterWaypoint, float> costFromStart,
        UnderwaterWaypoint target)
    {
        UnderwaterWaypoint best = open[0];
        float bestCost = TotalEstimatedCost(best, costFromStart, target);

        for (int index = 1; index < open.Count; index++)
        {
            UnderwaterWaypoint candidate = open[index];
            float candidateCost = TotalEstimatedCost(candidate, costFromStart, target);
            if (candidateCost < bestCost)
            {
                best = candidate;
                bestCost = candidateCost;
            }
        }

        return best;
    }

    private static float TotalEstimatedCost(
        UnderwaterWaypoint node,
        Dictionary<UnderwaterWaypoint, float> costFromStart,
        UnderwaterWaypoint target)
    {
        return costFromStart[node] + Vector3.Distance(node.transform.position, target.transform.position);
    }

    private static List<UnderwaterWaypoint> ReconstructPath(
        Dictionary<UnderwaterWaypoint, UnderwaterWaypoint> cameFrom,
        UnderwaterWaypoint current)
    {
        var result = new List<UnderwaterWaypoint> { current };
        while (cameFrom.TryGetValue(current, out UnderwaterWaypoint previous))
        {
            current = previous;
            result.Add(current);
        }

        result.Reverse();
        return result;
    }

    private void SmoothPath(Vector3 start, List<Vector3> path)
    {
        Vector3 anchor = start;
        int candidate = 1;

        while (candidate < path.Count)
        {
            if (HasClearPath(anchor, path[candidate]))
            {
                path.RemoveAt(candidate - 1);
            }
            else
            {
                anchor = path[candidate - 1];
                candidate++;
            }
        }
    }

    private static float FleeScore(Vector3 candidate, Vector3 start, Vector3 threat)
    {
        return Vector3.Distance(candidate, threat) - (0.2f * Vector3.Distance(candidate, start));
    }

    private void OnDrawGizmos()
    {
        if (drawConnectionsAlways)
        {
            DrawConnectionsGizmos();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!drawConnectionsAlways)
        {
            DrawConnectionsGizmos();
        }
    }

    private void DrawConnectionsGizmos()
    {
        if (!drawConnections)
        {
            return;
        }

        UnderwaterWaypoint[] editorWaypoints = GetComponentsInChildren<UnderwaterWaypoint>(true);
        Gizmos.color = new Color(0.1f, 0.8f, 1f, 0.35f);

        for (int first = 0; first < editorWaypoints.Length; first++)
        {
            for (int second = first + 1; second < editorWaypoints.Length; second++)
            {
                Vector3 a = editorWaypoints[first].transform.position;
                Vector3 b = editorWaypoints[second].transform.position;
                if (Vector3.Distance(a, b) <= maximumConnectionDistance && HasClearPath(a, b))
                {
                    Gizmos.DrawLine(a, b);
                }
            }
        }
    }
}
