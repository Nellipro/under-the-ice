using UnityEngine;

public class SwitchObject : MonoBehaviour
{
    public bool isOn;
    public float costPerSecond = 1; // Zet naar cost van object.
 

    public void ToggleObject(bool turnItOn)
    {
        if (turnItOn)
        {
            // set object online, either by gameobject enabeld or whatever...
        }
        else
        {
            // take object offline, either by gameobject disable or whatever...
        }

    for (int i = 0; i < transform.childCount; i++)
    {
        Transform child = transform.GetChild(i);
        child.gameObject.SetActive(turnItOn);
    }

    isOn = turnItOn;

    }
}
