using System.ComponentModel;
using UnityEngine;
using UnityEngine.AdaptivePerformance;

public class Reactor : MonoBehaviour
{
    public bool overLoaded;
    public float overLoadTimer;
    public float overLoadTimerMax;
    public float alarmBlinkSpeed;
    public float alarmBlinkTimer;
    bool alarmOn;
    public GameObject alarmLamp;
    public float lifeTimer;
    public float lifeTimerMax;
    public bool isFixed;
    public SodiumManeger sodiumManeger;
    public bool isExploding;

    void Awake()
    {
        overLoaded = false;
        isFixed = false;
        isExploding = false;
    }


    void Update()
    {
        isFixed = sodiumManeger.secondInteractionHappened;

        if (isFixed)
        {
            return;
        }
        
        lifeTimer += Time.deltaTime;

        alarmBlinkTimer += Time.deltaTime * 10;

        if (alarmBlinkTimer >= alarmBlinkSpeed & !alarmOn & overLoaded)
        {                        
            alarmLamp.SetActive(true);
            alarmBlinkTimer = 0;
            alarmOn = !alarmOn;
        }

        if (alarmBlinkTimer >= alarmBlinkSpeed & alarmOn & overLoaded)
        {
            alarmLamp.SetActive(false);
            alarmBlinkTimer = 0;
            alarmOn = !alarmOn;
        }

        if(overLoaded)
        {
            overLoadTimer += Time.deltaTime;

            if (overLoadTimer >= overLoadTimerMax)
            {
                isExploding = true;
            }            
        }

        if (lifeTimer >= lifeTimerMax)
        {
            overLoaded = true;
        }
    }

    public void Overload()
    {
        overLoaded = true;
    }
}
