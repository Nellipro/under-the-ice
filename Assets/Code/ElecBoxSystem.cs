using UnityEngine;
using UnityEngine.UIElements.Experimental;

public interface ISwitchValue
{
    float Value { get; }
    void TurnOff();
}
public interface IsPowerable
{
    void PowerOn();
    void PowerOf();
}

public class ElecBoxSystem : MonoBehaviour
{

    public static ElecBoxSystem Instance { get; private set; }
    
    public GameObject[] switches;
    public float[] switchValues;
    public GameObject[] connectedTo;

    [Header("alarm")]
    public GameObject alarmObject;
    public float alarmTimer = 0;
    public float maxAlarmTime = 0.5f;

    [Header("switchValues")]
    public float allValues;
    public float maxValues;

    [Header("elecBreak")]
    public float breakTimer = 0;
    public float maxBreakTime;



    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // Maak een class (lightholder oid) waarin zit : Array aan lights. Check of lights true of false (dus een return van bool)
    // Maak een lightswitch  management class, met daarin een array aan lightholders. Dus, lightswitchManager managed the lightholder. Lightholder switchen de lights erin.

    void Update()
    {
        allValues = 0;
        switchValues = new float[switches.Length];
        int switchIndex = 0;



        foreach (GameObject switchObject in switches)
        {
            if (switchObject == null || !TryGetSwitchValue(switchObject, out ISwitchValue switchValue))
            {
  
                switchIndex++;
                continue;
            }

            switchValues[switchIndex] = switchValue.Value;
            if (switchValue.Value >= 1f)
            {
                allValues++;
            }

            switchIndex++;
        }

//switch beaking
        if (allValues >= maxValues)
        {
            breakTimer += Time.deltaTime * ((allValues - maxValues) + 1);
            alarmTimer += Time.deltaTime;
            if (alarmTimer >= maxAlarmTime)
            {
                alarmObject.SetActive(true);
                alarmTimer = 0;
            }
            else if (alarmTimer >= maxAlarmTime * 0.5f)
            {
                alarmObject.SetActive(false);
            }

            if (breakTimer >= maxBreakTime)
            {
                foreach (GameObject switchObject in switches)
                {
                    if (switchObject != null && TryGetSwitchValue(switchObject, out ISwitchValue switchValueToTurnOff))
                    {
                        switchValueToTurnOff.TurnOff();
                    }
                }
                breakTimer = 0;
            }
        }
//
    }

    private bool TryGetSwitchValue(GameObject switchObject, out ISwitchValue switchValue)
    {
        foreach (MonoBehaviour component in switchObject.GetComponents<MonoBehaviour>())
        {
            if (component is ISwitchValue valueComponent)
            {
                switchValue = valueComponent;
                return true;
            }
        }

        switchValue = null;
        return false;
    }

    void PoweringOnAndOf()
    {
        float number = 0f;
        foreach (GameObject switchObject in switches)
        {
            if (switchObject != null)
            {
                TryGetSwitchValue(switchObject, out ISwitchValue switchValue);


            }
        }

        for (int switchIndex = 0; switchIndex < switches.Length; switchIndex++)
        {
            GameObject switchObject = switches[switchIndex];

            
            // if (number = switches[switchIndex])
            // {
            
            // }
        }   

    }
}
