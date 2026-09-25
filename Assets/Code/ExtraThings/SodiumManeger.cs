using UnityEngine;
using UnityEngine.Events;

public class SodiumManeger : MonoBehaviour, IInteractible
{
    Vector3 vector3Object;
    public GameObject sodiumPickUp;
    public GameObject sodiumDropPoint;
    public GameObject sodiumModel;
    [Space]
    [Space]
    [Space]
    [Space]
    [Space]
    [Space]
    [Space]
    [Space]
    public Transform startPos;
    public Transform endPos;
    public float moveSpeed = 8f;
    private Transform target;
    private bool hasInteracted;
    public bool secondInteractionHappened;

    [Header("Second interaction")]
    public UnityEvent onSecondInteraction;

    void Awake()
    {
        sodiumModel.SetActive(false);
        vector3Object = gameObject.transform.position;
        target = startPos.transform;  
    }

    void Update()
    {
        float step =  moveSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, target.position, step);    
    }

    public void Interact()
    {
        if (!hasInteracted)
        {
            hasInteracted = true;
            target = endPos;
            return;
        }

        secondInteractionHappened = true;
        onSecondInteraction?.Invoke();
    }

    public void Pull(Vector3 rayHitPoint)
    {
        
    }

    public void Release()
    {
        
    }

}
