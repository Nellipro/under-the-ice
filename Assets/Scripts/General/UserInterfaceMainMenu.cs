using UnityEngine;

public class UserInterfaceMainMenu : MonoBehaviour
{
    [SerializeField] private GameObject quitConfirmationDialog;
    public void PlayGame()
    {
        SceneFadeLoader.LoadScene(2);
    }
    public void ConfirmQuitDialog()
    {
        quitConfirmationDialog.SetActive(true);
    }
    public void CancelQuit()
    {
        quitConfirmationDialog.SetActive(false);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void Settings()
    {
        // Implement settings menu logic here
    }
    public void SettingsBack()
    {
        // Implement logic to go back from settings menu here
    }
}
