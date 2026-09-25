using UnityEngine;
using UnityEngine.InputSystem;
public class playerUI : MonoBehaviour
{
    [SerializeField] private bool showEscapeMenu = false;
    [SerializeField] private GameObject EscapeMenu;

    public bool ShowUI => showEscapeMenu; // Public property to access ShowUI
    
    public bool DebugInfoShown = false;
    [SerializeField] private GameObject DebugInfo;
    void OnDebug(InputValue value)
    {
        DebugInfoShown = !DebugInfoShown;
        DebugInfo.SetActive(DebugInfoShown);
    }
    void OnEscape(InputValue value)
    {
        showEscapeMenu = !showEscapeMenu;
    }
    // Update is called once per frame
    void Update()
    {
        if (showEscapeMenu)
        {
            PauseGame();
        }
        else if (!showEscapeMenu)
        {
            ContinueGame();
        }
    }
    void ContinueGame()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;
        EscapeMenu.SetActive(false);
    }
    void PauseGame()
    {
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        EscapeMenu.SetActive(true);

    }
}
