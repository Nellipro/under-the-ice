using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Paneltoggle : MonoBehaviour
{
    [SerializeField] private Animator panelAnimator;
    [SerializeField] private string openTrigger = "Open";
    [SerializeField] private string defaultTrigger = "Default";

    [SerializeField] private List<Slider> volumeSliders = new List<Slider>();
    private const string KeyPrefix = "Audio.Volume.";
    public void Start()
    {
        load();
    }
    public void OpenPanel()
    {
        PanelOn();
        Invoke("Test", 0.01f);
    }

    public void ReturnToDefault()
    {
        panelAnimator.ResetTrigger(openTrigger);
        panelAnimator.SetTrigger(defaultTrigger);
        Invoke("PanelOff", 1f); // Delay the deactivation to allow the animation to complete
    }
    public void PanelOn()
    {
        gameObject.SetActive(true);
    }
    public void PanelOff()
    {
        gameObject.SetActive(false);
        Debug.Log("Panel deactivated");
    }
    public void Test()
    {
        panelAnimator.ResetTrigger(defaultTrigger);
        panelAnimator.SetTrigger(openTrigger);
        
    }
    public void load()
    {
        for (int i = 0; i < volumeSliders.Count; i++)
        {
            Slider slider = volumeSliders[i];
            if (slider == null) continue;

            // If there isn't a saved value yet, use the slider's Inspector value.
            float savedValue = PlayerPrefs.GetFloat(KeyPrefix + i, slider.value);
            slider.value = savedValue;
        }
    }
    public void Save()
    {
         for (int i = 0; i < volumeSliders.Count; i++)
        {
            Slider slider = volumeSliders[i];
            if (slider == null) continue;

            PlayerPrefs.SetFloat(KeyPrefix + i, slider.value);
        }

        PlayerPrefs.Save();
    }

}
