using UnityEngine;
using UnityEngine.EventSystems;

public class UserInterfaceMainMenu : MonoBehaviour
{
    private GameObject previousSelection;
    public void PlayGame()
    {
        SceneFadeLoader.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    public void timescale(float time)
    {
        Time.timeScale = time;
    }
}
