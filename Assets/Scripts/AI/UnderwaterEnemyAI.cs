using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public sealed class UnderwaterEnemyAI : MonoBehaviour
{
    public enum EnemyBehaviour
    {
        ChasesPlayerAndFearsSonar,
        IgnoresPlayerAndAttacksSonar
    }

    public enum EnemyState
    {
        Patrol,
        ChasePlayer,
        FleeSonar,
        ChaseSonar,
        AttackPlayer,
        AttackSonar
    }

    [Header("Role")]
    [SerializeField] private EnemyBehaviour behaviour;
    [SerializeField] private WaypointNetwork3D waypointNetwork;
    [Tooltip("Assign the player's root transform. The current player prefab is not tagged Player.")]
    [SerializeField] private Transform playerTarget;

    [Header("Movement")]
    [SerializeField, Min(0.1f)] private float swimSpeed = 6f;
    [SerializeField, Min(0.1f)] private float acceleration = 8f;
    [SerializeField, Min(0.1f)] private float turnSpeed = 4f;
    [SerializeField, Min(0.05f)] private float waypointTolerance = 1f;
    [SerializeField, Min(0.1f)] private float pathRefreshInterval = 0.75f;

    [Header("Player Hunter")]
    [SerializeField, Min(0f)] private float playerDetectionDistance = 35f;
    [SerializeField, Min(0f)] private float sonarFearDistance = 25f;
    [SerializeField, Min(0f)] private float playerAttackDistance = 2f;

    [Header("Sonar Hunter")]
    [SerializeField, Min(0f)] private float sonarDetectionDistance = 40f;
    [SerializeField, Min(0f)] private float sonarLoseDistance = 55f;
    [SerializeField, Min(0f)] private float sonarAttackDistance = 2.5f;

    [Header("Attacks")]
    [SerializeField, Min(0.05f)] private float attackInterval = 1f;
    [SerializeField] private UnityEvent onPlayerAttack;
    [SerializeField] private UnityEvent onSonarAttack;

    [Header("Debug")]
    [SerializeField] private EnemyState currentState;
    [SerializeField] private bool drawCurrentPath = true;

    private Rigidbody body;
    private SonarStimulus sonarTarget;
    private readonly List<Vector3> currentPath = new();
    private int pathIndex;
    private float nextPathRefreshTime;
    private float nextAttackTime;
    private Vector3 patrolDestination;
    private bool hasPatrolDestination;

    public EnemyState CurrentState => currentState;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        body.useGravity = false;
        SetState(EnemyState.Patrol);
    }

    private void Update()
    {
        EvaluateBehaviour();

        if (IsAttackState(currentState))
        {
            PerformAttack();
            return;
        }

        if (Time.time >= nextPathRefreshTime || currentPath.Count == 0)
        {
            RefreshPath();
            nextPathRefreshTime = Time.time + pathRefreshInterval;
        }
    }

    private void FixedUpdate()
    {
        if (IsAttackState(currentState) || currentPath.Count == 0 || pathIndex >= currentPath.Count)
        {
            SlowDown();
            return;
        }

        Vector3 toWaypoint = currentPath[pathIndex] - body.position;
        if (toWaypoint.magnitude <= waypointTolerance)
        {
            pathIndex++;
            if (pathIndex >= currentPath.Count)
            {
                if (currentState == EnemyState.Patrol)
                {
                    hasPatrolDestination = false;
                }

                SlowDown();
                return;
            }

            toWaypoint = currentPath[pathIndex] - body.position;
        }

        Vector3 direction = toWaypoint.normalized;
        Vector3 desiredVelocity = direction * swimSpeed;
        body.linearVelocity = Vector3.MoveTowards(
            body.linearVelocity,
            desiredVelocity,
            acceleration * Time.fixedDeltaTime);

        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion desiredRotation = Quaternion.LookRotation(direction, Vector3.up);
            body.MoveRotation(Quaternion.Slerp(
                body.rotation,
                desiredRotation,
                turnSpeed * Time.fixedDeltaTime));
        }
    }

    private void EvaluateBehaviour()
    {
        if (behaviour == EnemyBehaviour.ChasesPlayerAndFearsSonar)
        {
            EvaluatePlayerHunter();
        }
        else
        {
            EvaluateSonarHunter();
        }
    }

    private void EvaluatePlayerHunter()
    {
        SonarStimulus frighteningSonar = FindClosestEmittingSonar(sonarFearDistance, false);
        if (frighteningSonar != null)
        {
            sonarTarget = frighteningSonar;
            SetState(EnemyState.FleeSonar);
            return;
        }

        sonarTarget = null;
        if (playerTarget == null || Vector3.Distance(transform.position, playerTarget.position) > playerDetectionDistance)
        {
            SetState(EnemyState.Patrol);
            return;
        }

        SetState(Vector3.Distance(transform.position, playerTarget.position) <= playerAttackDistance
            ? EnemyState.AttackPlayer
            : EnemyState.ChasePlayer);
    }

    private void EvaluateSonarHunter()
    {
        if (sonarTarget == null || !sonarTarget.IsEmitting ||
            Vector3.Distance(transform.position, sonarTarget.AttackPosition) > sonarLoseDistance)
        {
            sonarTarget = FindClosestEmittingSonar(sonarDetectionDistance, true);
        }

        if (sonarTarget == null)
        {
            SetState(EnemyState.Patrol);
            return;
        }

        SetState(Vector3.Distance(transform.position, sonarTarget.AttackPosition) <= sonarAttackDistance
            ? EnemyState.AttackSonar
            : EnemyState.ChaseSonar);
    }

    private SonarStimulus FindClosestEmittingSonar(float maximumDistance, bool requireLineOfSight)
    {
        SonarStimulus closest = null;
        float closestSquaredDistance = maximumDistance * maximumDistance;

        foreach (SonarStimulus sonar in SonarStimulus.Instances)
        {
            if (sonar == null || !sonar.IsEmitting)
            {
                continue;
            }

            float squaredDistance = (sonar.AttackPosition - transform.position).sqrMagnitude;
            if (squaredDistance >= closestSquaredDistance)
            {
                continue;
            }

            if (requireLineOfSight && waypointNetwork != null &&
                !waypointNetwork.HasClearPath(transform.position, sonar.AttackPosition))
            {
                continue;
            }

            closest = sonar;
            closestSquaredDistance = squaredDistance;
        }

        return closest;
    }

    private void RefreshPath()
    {
        List<Vector3> newPath;

        switch (currentState)
        {
            case EnemyState.FleeSonar:
                if (sonarTarget == null)
                {
                    return;
                }

                newPath = waypointNetwork != null
                    ? waypointNetwork.FindFleePath(transform.position, sonarTarget.AttackPosition)
                    : DirectFleePath(sonarTarget.AttackPosition);
                break;

            case EnemyState.ChasePlayer:
                if (playerTarget == null)
                {
                    return;
                }

                newPath = FindPath(playerTarget.position);
                break;

            case EnemyState.ChaseSonar:
                if (sonarTarget == null)
                {
                    return;
                }

                newPath = FindPath(sonarTarget.AttackPosition);
                break;

            default:
                if (!hasPatrolDestination)
                {
                    patrolDestination = waypointNetwork != null
                        ? waypointNetwork.GetRandomWaypointPosition(transform.position)
                        : transform.position;
                    hasPatrolDestination = true;
                }

                newPath = FindPath(patrolDestination);
                break;
        }

        currentPath.Clear();
        currentPath.AddRange(newPath);
        pathIndex = 0;
    }

    private List<Vector3> FindPath(Vector3 destination)
    {
        return waypointNetwork != null
            ? waypointNetwork.FindPath(transform.position, destination)
            : new List<Vector3> { destination };
    }

    private List<Vector3> DirectFleePath(Vector3 threatPosition)
    {
        Vector3 away = (transform.position - threatPosition).normalized;
        if (away.sqrMagnitude < 0.001f)
        {
            away = transform.forward;
        }

        return new List<Vector3> { transform.position + (away * sonarFearDistance) };
    }

    private void PerformAttack()
    {
        SlowDown();
        FaceAttackTarget();

        if (Time.time < nextAttackTime)
        {
            return;
        }

        nextAttackTime = Time.time + attackInterval;

        if (currentState == EnemyState.AttackPlayer)
        {
            onPlayerAttack.Invoke();
        }
        else if (currentState == EnemyState.AttackSonar && sonarTarget != null)
        {
            sonarTarget.ReceiveAttack();
            onSonarAttack.Invoke();
        }
    }

    private void FaceAttackTarget()
    {
        Vector3 target = currentState == EnemyState.AttackPlayer && playerTarget != null
            ? playerTarget.position
            : sonarTarget != null ? sonarTarget.AttackPosition : transform.position;

        Vector3 direction = (target - transform.position).normalized;
        if (direction.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(direction, Vector3.up),
                turnSpeed * Time.deltaTime);
        }
    }

    private void SlowDown()
    {
        body.linearVelocity = Vector3.MoveTowards(
            body.linearVelocity,
            Vector3.zero,
            acceleration * Time.fixedDeltaTime);
    }

    private void SetState(EnemyState newState)
    {
        if (currentState == newState)
        {
            return;
        }

        currentState = newState;
        currentPath.Clear();
        pathIndex = 0;
        nextPathRefreshTime = 0f;

        if (newState == EnemyState.Patrol)
        {
            hasPatrolDestination = false;
        }
    }

    private static bool IsAttackState(EnemyState state)
    {
        return state == EnemyState.AttackPlayer || state == EnemyState.AttackSonar;
    }

    private void OnDrawGizmosSelected()
    {
        if (!drawCurrentPath || currentPath.Count == 0)
        {
            return;
        }

        Gizmos.color = Color.yellow;
        Vector3 previous = transform.position;
        for (int index = pathIndex; index < currentPath.Count; index++)
        {
            Gizmos.DrawLine(previous, currentPath[index]);
            Gizmos.DrawSphere(currentPath[index], 0.15f);
            previous = currentPath[index];
        }
    }
}
