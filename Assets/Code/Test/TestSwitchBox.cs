using System.Diagnostics;
using UnityEngine;

public class TestSwitchBox : MonoBehaviour
{
    public SwitchObject[] switchObjects;
    public float currentPowerUse;
    public float maxPowerUse = 5;

    void Update()
    {
        CalculateUpkeep();
        if(currentPowerUse >= maxPowerUse)
        {
            ShutDownAllSwitchableObjects();
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
