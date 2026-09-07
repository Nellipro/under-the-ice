using System.Diagnostics;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class TestSwitchBox : MonoBehaviour
{
    public SwitchObject[] switchObjects;
    public SwitchSwitchSystem[] switches;
    public GameObject alarmLamp;
    public float currentPowerUse;
    public float maxPowerUse;
    public float maxTimeTimer;
    public float timer;
    public float alarmBlinkSpeed;
    public float alarmBlinkTimer;
    bool alarmOn;



    void Awake()
    {
        timer = 0;
    }

    void Update()
    {
        CalculateUpkeep();

        if(currentPowerUse >= maxPowerUse)
        {
            timer += Time.deltaTime;
            alarmBlinkTimer += Time.deltaTime * 10;

            if (alarmBlinkTimer >= alarmBlinkSpeed & !alarmOn)
            {
                alarmLamp.SetActive(true);
                alarmBlinkTimer = 0;
                alarmOn = !alarmOn;

            }
            if (alarmBlinkTimer >= alarmBlinkSpeed & alarmOn)
            {
                alarmLamp.SetActive(false);
                alarmBlinkTimer = 0;
                alarmOn = !alarmOn;
            }

            if (timer >= maxTimeTimer)
            {
                ShutDownAllSwitchableObjects();             
                timer = 0;
            }
        }

        if(currentPowerUse <= maxPowerUse)
        {
            alarmLamp.SetActive(false);

 
        }
    }

    public void CalculateUpkeep()
    {
        float powerUse = 0;
        foreach(SwitchObject s in switchObjects)
        {
            if (s.isOn)
            {
                powerUse += s.costPerSecond;
            }
        }
        currentPowerUse = powerUse;
    }

    public void ShutDownAllSwitchableObjects()
    {
        // Als dit over tijd moet, maar IEnumerator en doe een yield WaitForSeconds hier oid.
        foreach(SwitchObject s in switchObjects)
        {
            s.ToggleObject(false);
        }

        foreach(SwitchSwitchSystem s in switches)
        {
            s.TurnOff();
        }
    }

    public void EnableSpecificSwitchableObject(SwitchObject objectToEnable)
    {
        objectToEnable.ToggleObject(true);
    }
    
    public void DisableSpecificSwitchableObject(SwitchObject objectToEnable)
    {
        objectToEnable.ToggleObject(false);
    }

}
