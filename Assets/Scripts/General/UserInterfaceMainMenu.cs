using UnityEngine;
using UnityEngine.EventSystems;

public class UserInterfaceMainMenu : MonoBehaviour
{
    [SerializeField] private GameObject quitConfirmationDialog;
    [SerializeField] private GameObject quitDialogFirstSelected;
    [SerializeField] private GameObject QuitSelection;
    [SerializeField] private GameObject FirstSelected;
    private GameObject previousSelection;
    public void PlayGame()
    {
        SceneFadeLoader.LoadScene(2);
    }
    public void ConfirmQuitDialog()
    {
        previousSelection = EventSystem.current.currentSelectedGameObject;
        quitConfirmationDialog.SetActive(true);
        EventSystem.current.SetSelectedGameObject(quitDialogFirstSelected);
    }
    public void CancelQuit()
    {
        quitConfirmationDialog.SetActive(false);
        EventSystem.current.SetSelectedGameObject(previousSelection);
        EventSystem.current.SetSelectedGameObject(QuitSelection);
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
    public void ResetInput()
    {
        EventSystem.current.SetSelectedGameObject(FirstSelected);
    }
}
