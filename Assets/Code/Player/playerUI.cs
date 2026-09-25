using UnityEngine;
using UnityEngine.InputSystem;
public class playerUI : MonoBehaviour
{
    [SerializeField] private bool showEscapeMenu = false;
    [SerializeField] private GameObject EscapeMenu;
    private bool cameraViewerOpen = false;

    public bool ShowUI => showEscapeMenu || cameraViewerOpen; // Public property to access ShowUI

    public void SetCameraViewerOpen(bool isOpen)
    {
        cameraViewerOpen = isOpen;
    }
    
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
        else if (cameraViewerOpen)
        {
            ShowCameraViewer();
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

    void ShowCameraViewer()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        EscapeMenu.SetActive(false);
    }
}
