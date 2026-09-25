using System.Collections;
using UnityEngine;
using System.Collections.Generic;


public class SonarSystem : MonoBehaviour
{
    [Header("Sonar pulse")]
    [SerializeField] private Transform sonarOrigin;
    [SerializeField] private GameObject pulsePrefab;
    [SerializeField] private float sonarRange;
    [SerializeField] private float pulseDuration;
    [Min(0.01f)]
    [SerializeField] private float speedModifier = 1f;
    [SerializeField] private float pulseCooldown;
    [SerializeField] private LayerMask detectableLayers;
    [SerializeField] private int latitudeLines;
    [SerializeField] private int longitudePoints;
    [SerializeField] private GameObject hitPrefab;
    private float hitPrefabLifetime;
    private float cooldownTimer;



    void Awake()
    {
        hitPrefabLifetime = pulseDuration + 1;
    }

    private void Update()
    {
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }
    }

    public void OnSonar()
    {   
        SendPulse();        
    }

    public void SendPulse()
    {
        if (cooldownTimer > 0f)
        {
            return;
        }

        cooldownTimer = pulseCooldown;
        Vector3 origin = sonarOrigin != null ? sonarOrigin.position : transform.position;

        if (pulsePrefab != null)
        {
            GameObject pulse = Instantiate(pulsePrefab, origin, Quaternion.identity);
            StartCoroutine(ExpandPulse(pulse));
        }
    }

    private IEnumerator ExpandPulse(GameObject pulse)
    {
        Vector3 origin = sonarOrigin != null
            ? sonarOrigin.position
            : transform.position;

        List<Vector3> directions = new List<Vector3>();
        List<bool> stoppedPings = new List<bool>();

        for (int latitude = 0; latitude <= latitudeLines; latitude++)
        {
            float verticalRadians =
                Mathf.Lerp(-90f, 90f, (float)latitude / latitudeLines)
                * Mathf.Deg2Rad;

            float y = Mathf.Sin(verticalRadians);
            float horizontalRadius = Mathf.Cos(verticalRadians);

            for (int longitude = 0; longitude < longitudePoints; longitude++)
            {
                float horizontalRadians =
                    longitude * 360f / longitudePoints * Mathf.Deg2Rad;

                Vector3 direction = new Vector3(
                    Mathf.Cos(horizontalRadians) * horizontalRadius,
                    y,
                    Mathf.Sin(horizontalRadians) * horizontalRadius
                );

                directions.Add(direction);
                stoppedPings.Add(false);
            }
        }

        float elapsed = 0f;
        float previousRadius = 0f;

        while (elapsed < pulseDuration)
        {
            elapsed += Time.deltaTime * speedModifier;

            float progress = Mathf.Clamp01(elapsed / pulseDuration);
            float radius = sonarRange * progress;

            pulse.transform.position = origin;
            pulse.transform.localScale = Vector3.one * radius * 2f;

            for (int i = 0; i < directions.Count; i++)
            {
                if (stoppedPings[i])
                {
                    continue;
                }

                Vector3 previousPosition =
                    origin + directions[i] * previousRadius;

                Vector3 nextPosition =
                    origin + directions[i] * radius;

                Vector3 movement = nextPosition - previousPosition;

                if (movement.sqrMagnitude > 0f &&
                    Physics.Raycast(
                        previousPosition,
                        movement.normalized,
                        out RaycastHit hit,
                        movement.magnitude,
                        detectableLayers,
                        QueryTriggerInteraction.Ignore) &&
                    hit.transform.root != transform.root)
                {
                    stoppedPings[i] = true;

                    if (hitPrefab != null)
                    {
                        GameObject hitEffect = Instantiate(
                            hitPrefab,
                            hit.point,
                            Quaternion.LookRotation(hit.normal)
                        );

                        Destroy(hitEffect, hitPrefabLifetime);
                    }

                    Debug.Log("Sonar ping detected: " + hit.collider.name);
                }
            }

            previousRadius = radius;
            yield return null;
        }

        Destroy(pulse);
    }

    private void OnDrawGizmosSelected()
    {
        Transform origin = sonarOrigin != null ? sonarOrigin : transform;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(origin.position, sonarRange);
    }

    


}
