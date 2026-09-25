using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SecurityCamras : MonoBehaviour, IInteractible
{
    [Header("Camera Feeds")]
    [SerializeField] private Camera[] cameraFeeds;
    [SerializeField] private RawImage cameraDisplay;
    [SerializeField] private GameObject viewerRoot;

    [Header("Controls")]
    [SerializeField] private Button previousButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button closeButton;

    [Header("Player")]
    [SerializeField] private playerUI playerUI;

    private RenderTexture cameraTexture;
    private RenderTexture[] originalTargets;
    private bool[] originalCameraStates;
    private Camera activeCamera;
    private int activeCameraIndex;
    private bool viewerOpen;
    private CursorLockMode previousCursorLockState;
    private bool previousCursorVisibility;

    private void Awake()
    {
        if (cameraFeeds == null || cameraFeeds.Length == 0)
        {
            return;
        }

        cameraTexture = new RenderTexture(Screen.width, Screen.height, 24)
        {
            name = "Security Camera Feed"
        };

        originalTargets = new RenderTexture[cameraFeeds.Length];
        originalCameraStates = new bool[cameraFeeds.Length];

        if (viewerRoot != null)
        {
            viewerRoot.SetActive(false);
        }
    }

    private void Update()
    {
        if (!viewerOpen || Keyboard.current == null)
        {
            return;
        }

        if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            ShowPreviousCamera();
        }
        else if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            ShowNextCamera();
        }
        else if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            CloseViewer();
        }
    }

    public void Interact()
    {
        if (viewerOpen || cameraFeeds == null || cameraFeeds.Length == 0)
        {
            return;
        }

        OpenViewer();
    }

    public void ShowPreviousCamera()
    {
        if (!viewerOpen || cameraFeeds.Length == 0)
        {
            return;
        }

        activeCameraIndex = (activeCameraIndex - 1 + cameraFeeds.Length) % cameraFeeds.Length;
        SetActiveCamera();
    }

    public void ShowNextCamera()
    {
        if (!viewerOpen || cameraFeeds.Length == 0)
        {
            return;
        }

        activeCameraIndex = (activeCameraIndex + 1) % cameraFeeds.Length;
        SetActiveCamera();
    }

    public void CloseViewer()
    {
        if (!viewerOpen)
        {
            return;
        }

        viewerOpen = false;

        for (int i = 0; i < cameraFeeds.Length; i++)
        {
            if (cameraFeeds[i] == null)
            {
                continue;
            }

            cameraFeeds[i].targetTexture = originalTargets[i];
            cameraFeeds[i].enabled = originalCameraStates[i];
        }

        if (viewerRoot != null)
        {
            viewerRoot.SetActive(false);
        }

        if (playerUI != null)
        {
            playerUI.SetCameraViewerOpen(false);
        }

        Cursor.lockState = previousCursorLockState;
        Cursor.visible = previousCursorVisibility;
    }

    public void Pull(Vector3 rayHitPoint)
    {
    }

    public void Release()
    {
    }

    private void OpenViewer()
    {
        viewerOpen = true;
        previousCursorLockState = Cursor.lockState;
        previousCursorVisibility = Cursor.visible;

        for (int i = 0; i < cameraFeeds.Length; i++)
        {
            if (cameraFeeds[i] == null)
            {
                continue;
            }

            originalTargets[i] = cameraFeeds[i].targetTexture;
            originalCameraStates[i] = cameraFeeds[i].enabled;
            cameraFeeds[i].enabled = false;
        }

        if (viewerRoot != null)
        {
            viewerRoot.SetActive(true);
        }

        if (cameraDisplay != null)
        {
            cameraDisplay.texture = cameraTexture;
        }

        if (previousButton != null)
        {
            previousButton.onClick.AddListener(ShowPreviousCamera);
        }

        if (nextButton != null)
        {
            nextButton.onClick.AddListener(ShowNextCamera);
        }

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseViewer);
        }

        if (playerUI != null)
        {
            playerUI.SetCameraViewerOpen(true);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SetActiveCamera();
    }

    private void SetActiveCamera()
    {
        if (activeCamera != null)
        {
            activeCamera.targetTexture = null;
            activeCamera.enabled = false;
        }

        activeCamera = cameraFeeds[activeCameraIndex];

        if (activeCamera == null)
        {
            return;
        }

        activeCamera.targetTexture = cameraTexture;
        activeCamera.enabled = true;
    }

    private void OnDisable()
    {
        if (viewerOpen)
        {
            CloseViewer();
        }

        if (previousButton != null)
        {
            previousButton.onClick.RemoveListener(ShowPreviousCamera);
        }

        if (nextButton != null)
        {
            nextButton.onClick.RemoveListener(ShowNextCamera);
        }

        if (closeButton != null)
        {
            closeButton.onClick.RemoveListener(CloseViewer);
        }

        if (cameraTexture != null)
        {
            cameraTexture.Release();
            Destroy(cameraTexture);
        }
    }
}
