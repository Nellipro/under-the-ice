using System.Data.Common;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;

public class SwitchObject : MonoBehaviour
{
    Vector3 vector3Object;
    public bool isOn;
    public float costPerSecond = 1;

    [Header("What Am i?")]
    public bool isLamp;
    public bool isDoor_AlsoForAirLock;
    public bool isDoorReverst;
    public bool isEngine;
    public bool isBlastShield;
    public bool isReactor;


    [Header("for door and blastshield")]
    public Transform openPos;
    public Transform closedPos;
    public float moveSpeed = 8f;
    private Transform target;

    [Header("for Engine")]
    public SubmarineMovement submarine;

    [Header("for BlastsShield")]
    public float rotationSpeed = 90f;

    [Header("for Reactor")]
    public Reactor reactor;






    void Awake()
    {
        if (isDoor_AlsoForAirLock)
        {
            vector3Object = gameObject.transform.position;
            target = closedPos.transform;
        }
        if (isBlastShield)
        {
            isDoor_AlsoForAirLock = true;
        }
    }

    void Update()
    {
        if (isDoor_AlsoForAirLock)
        {
            float step =  moveSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, target.position, step);
        }

        if (isBlastShield)
        {
            float step = rotationSpeed * Time.deltaTime;
            transform.rotation = Quaternion.RotateTowards(transform.rotation,target.rotation,step);
        }
    }

    public void ToggleObject(bool turnItOn)
    {
        if (turnItOn)
        {
            if(isDoor_AlsoForAirLock & !isDoorReverst)
            {
                target = openPos.transform;
            }
            if(isDoorReverst)
            {
                target = closedPos.transform;
            }

            if(isEngine)
            {
                submarine.isActive = true;
            }

            if(isReactor)
            {
                reactor.overLoaded = true;
            }

            
        }
        else
        {
            if(isDoor_AlsoForAirLock & !isDoorReverst)
            {
               target = closedPos.transform;
            }

            if(isDoorReverst)
            {
                target = openPos.transform;
            }

            if(isEngine)
            {
                submarine.isActive = false;
            }
        }


        if (isLamp)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                child.gameObject.SetActive(turnItOn);
            }
        }
    

        isOn = turnItOn;

    }
}
