using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneFadeLoader : MonoBehaviour
{
    public static SceneFadeLoader Instance { get; private set; }

    [Header("Fade UI")]
    [SerializeField] private CanvasGroup fadeCanvasGroup;

    [Header("Timing")]
    [SerializeField] private float fadeToBlackDuration = 0.35f;
    [SerializeField] private float fadeFromBlackDuration = 0.45f;
    [SerializeField] private float holdBlackBeforeFadeOut = 1f;

    [Header("Startup")]
    [SerializeField] private bool startFullyBlack = true;
    [SerializeField] private bool fadeInOnStartup = false;

    private bool isTransitioning;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (fadeCanvasGroup == null)
        {
            fadeCanvasGroup = GetComponentInChildren<CanvasGroup>(true);
        }

        if (fadeCanvasGroup == null)
        {
            Debug.LogError("SceneFadeLoader requires a CanvasGroup reference to a full-screen black UI panel.");
            return;
        }

        fadeCanvasGroup.alpha = startFullyBlack ? 1f : 0f;
        fadeCanvasGroup.blocksRaycasts = fadeCanvasGroup.alpha > 0.001f;
        fadeCanvasGroup.interactable = false;
    }

    private IEnumerator Start()
    {
        if (fadeInOnStartup)
        {
            yield return HoldAtBlackIfNeeded();
            yield return FadeTo(0f, fadeFromBlackDuration);
        }
    }

    public static void LoadScene(int buildIndex)
    {
        if (Instance == null)
        {
            SceneManager.LoadScene(buildIndex);
            return;
        }

        Instance.StartTransition(Instance.TransitionToSceneByIndex(buildIndex));
    } 

    public static void LoadScene(string sceneName)
    {
        if (Instance == null)
        {
            SceneManager.LoadScene(sceneName);
            return;
        }

        Instance.StartTransition(Instance.TransitionToSceneByName(sceneName));
    }

    private void StartTransition(IEnumerator transitionRoutine)
    {
        if (isTransitioning)
        {
            return;
        }

        StartCoroutine(transitionRoutine);
    }

    private IEnumerator TransitionToSceneByIndex(int buildIndex)
    {
        isTransitioning = true;

        yield return FadeTo(1f, fadeToBlackDuration);

        AsyncOperation loadOp = SceneManager.LoadSceneAsync(buildIndex);
        while (!loadOp.isDone)
        {
            yield return null;
        }

        yield return null;
        yield return HoldAtBlackIfNeeded();
        yield return FadeTo(0f, fadeFromBlackDuration);

        isTransitioning = false;
    }

    private IEnumerator TransitionToSceneByName(string sceneName)
    {
        isTransitioning = true;

        yield return FadeTo(1f, fadeToBlackDuration);

        AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneName);
        while (!loadOp.isDone)
        {
            yield return null;
        }

        yield return null;
        yield return HoldAtBlackIfNeeded();
        yield return FadeTo(0f, fadeFromBlackDuration);

        isTransitioning = false;
    }

    private IEnumerator FadeTo(float targetAlpha, float duration)
    {
        if (fadeCanvasGroup == null)
        {
            yield break;
        }

        float startAlpha = fadeCanvasGroup.alpha;

        if (duration <= 0f)
        {
            fadeCanvasGroup.alpha = targetAlpha;
            fadeCanvasGroup.blocksRaycasts = targetAlpha > 0.001f;
            yield break;
        }

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            fadeCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            yield return null;
        }

        fadeCanvasGroup.alpha = targetAlpha;
        fadeCanvasGroup.blocksRaycasts = targetAlpha > 0.001f;
    }

    private IEnumerator HoldAtBlackIfNeeded()
    {
        if (holdBlackBeforeFadeOut <= 0f)
        {
            yield break;
        }

        yield return new WaitForSecondsRealtime(holdBlackBeforeFadeOut);
    }
}
