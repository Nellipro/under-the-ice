using UnityEngine;

public class Paneltoggle : MonoBehaviour
{
    [SerializeField] private Animator panelAnimator;
    [SerializeField] private string openTrigger = "Open";
    [SerializeField] private string defaultTrigger = "Default";

    public void OpenPanel()
    {
        PanelOn();
        Invoke("Test", 0.01f);
    }

    public void ReturnToDefault()
    {
        panelAnimator.ResetTrigger(openTrigger);
        panelAnimator.SetTrigger(defaultTrigger);
        Invoke("PanelOff", 1.01f); // Delay the deactivation to allow the animation to complete
    }
    public void PanelOn()
    {
        gameObject.SetActive(true);
    }
    public void PanelOff()
    {
        gameObject.SetActive(false);
    }
    public void Test()
    {
        panelAnimator.ResetTrigger(defaultTrigger);
        panelAnimator.SetTrigger(openTrigger);
        
    }
}
