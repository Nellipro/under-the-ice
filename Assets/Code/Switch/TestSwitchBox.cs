using System.Diagnostics;
using UnityEngine;

public class TestSwitchBox : MonoBehaviour
{
    public SwitchObject[] switchObjects;
    public SwitchSwitchSystem[] switches;
    public float currentPowerUse;
    public float maxPowerUse = 5;
    public float maxTimeTimer = 0;
    public float timer = 0f;



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

            if (timer >= maxTimeTimer)
            {
                ShutDownAllSwitchableObjects();

                timer = 0;
            }
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
