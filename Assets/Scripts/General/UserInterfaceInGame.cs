using UnityEngine;
using UnityEngine.EventSystems;
public class UserInterfaceInGame : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuDialog; 
    [SerializeField] private GameObject escMenuDialog; 
    [SerializeField] private GameObject escMenuFirstSelected;
    public void MainMenuDialog()
    {
        mainMenuDialog.SetActive(true); // Show the main menu dialog
        EventSystem.current.SetSelectedGameObject(mainMenuDialog); // Set the selected game object to the main menu dialog
    }
    public void CloseMainMenuDialog()
    {
        mainMenuDialog.SetActive(false); // Hide the main menu dialog
    }
    public void backToMainMenu()
    {
        SceneFadeLoader.LoadScene(1); // Load the main menu scene (assuming scene index 0 is the main menu)
    }
    public void closeEscMenu()
    {
        escMenuDialog.SetActive(false); // Hide the escape menu dialog
        EventSystem.current.SetSelectedGameObject(escMenuFirstSelected);
    }
}
