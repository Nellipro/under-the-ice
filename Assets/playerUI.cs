using UnityEngine;

public class playerUI : MonoBehaviour
{
    [SerializeField] private GameObject UICanvas;
    [SerializeField] private bool ShowUI = false;
    [SerializeField] private bool TimeStop = false;
    [SerializeField] private AnimationCurve slowdownCurve;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void StartSlowdown(float duration)
    {
        if (slowdownRoutine != null)
            StopCoroutine(slowdownRoutine);
        slowdownRoutine = StartCoroutine(SlowTimeToZero(duration));
    }
    
    private IEnumerator SlowTimeToZero(float duration)
    {
        float startScale = Time.timeScale;
        float elapsed = 0f;
    
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime; // unaffected by timeScale itself
            Time.timeScale = Mathf.Lerp(startScale, 0f, elapsed / duration);
            yield return null;
        }
    
        Time.timeScale = 0f;
    }
}
