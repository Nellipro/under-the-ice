using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Add this to the sonar object. The existing sonar controls should call
/// SetEmitting(true) when the sonar is active and SetEmitting(false) when it stops.
/// </summary>
public sealed class SonarStimulus : MonoBehaviour
{
    private static readonly List<SonarStimulus> ActiveInstances = new();

    [SerializeField] private bool isEmitting;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private UnityEvent onAttacked;

    public bool IsEmitting => isActiveAndEnabled && isEmitting;
    public Vector3 AttackPosition => attackPoint != null ? attackPoint.position : transform.position;
    public static IReadOnlyList<SonarStimulus> Instances => ActiveInstances;

    private void OnEnable()
    {
        if (!ActiveInstances.Contains(this))
        {
            ActiveInstances.Add(this);
        }
    }

    private void OnDisable()
    {
        ActiveInstances.Remove(this);
    }

    public void SetEmitting(bool value)
    {
        isEmitting = value;
    }

    public void ReceiveAttack()
    {
        onAttacked.Invoke();
    }
}
