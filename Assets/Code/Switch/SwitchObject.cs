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
    public bool isDoor;
    public bool isEngine;

    [Header("for door")]
    public Transform openPos;
    public Transform closedPos;
    public float moveSpeed = 8f;
    private Transform target;

    [Header("for Engine")]
    public SubmarineMovement submarine;



    void Awake()
    {
        if (isDoor)
        {
            vector3Object = gameObject.transform.position;
            target = closedPos.transform;
        }
    }

    void Update()
    {
        if (isDoor)
        {
            float step =  moveSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, target.position, step);
        }
    }

    public void ToggleObject(bool turnItOn)
    {
        if (turnItOn)
        {
            if(isDoor)
            {
                target = openPos.transform;
            }

            if(isEngine)
            {
                submarine.isActive = true;
            }
        }
        else
        {
            if(isDoor)
            {
               target = closedPos.transform;
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
