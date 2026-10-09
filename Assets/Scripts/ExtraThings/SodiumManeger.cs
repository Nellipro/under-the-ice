using UnityEngine;
using UnityEngine.Events;

public class SodiumManeger : MonoBehaviour
{
    Vector3 vector3Object;
    public GameObject sodiumPickUp;
    public GameObject sodiumDropPoint;
    public GameObject sodiumModel;
    public bool secondInteractionHappened;
    [Space]
    [Space]
    [Space]
    public Transform startPos;
    public Transform endPos;
    public float moveSpeed = 8f;
    private Transform target;

    [Header("Second interaction")]
    public UnityEvent onSecondInteraction;

    void Awake()
    {
        vector3Object = gameObject.transform.position;
        target = endPos.transform;
        if (sodiumModel != null)
        {
            sodiumModel.SetActive(false);
        }
    }

    void Update()
    {
        float step =  moveSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, target.position, step);

        if (secondInteractionHappened)
        {
            target = startPos.transform;
        }
        else if (!secondInteractionHappened)
        {
            target = endPos.transform;
        }
    }

    public void PickUp()
    {
        if (secondInteractionHappened || sodiumModel == null)
        {
            return;
        }

        sodiumModel.SetActive(true);
        if (sodiumPickUp != null)
        {
            sodiumPickUp.SetActive(false);
        }
    }

    public void Place()
    {
        if (secondInteractionHappened || sodiumModel == null || !sodiumModel.activeSelf)
        {
            return;
        }

        if (sodiumDropPoint != null)
        {
            sodiumModel.transform.SetPositionAndRotation(
                sodiumDropPoint.transform.position,
                sodiumDropPoint.transform.rotation);
        }

        secondInteractionHappened = true;
        onSecondInteraction?.Invoke();
    }

}
