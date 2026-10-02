using UnityEngine;

public class MainSceneLoader : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        SceneFadeLoader.LoadScene(2);
    }
}
