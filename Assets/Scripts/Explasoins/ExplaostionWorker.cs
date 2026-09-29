using UnityEngine;
using UnityEngine.SceneManagement;

public class ExplaostionWorker : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            SceneFadeLoader.LoadScene(SceneManager.GetActiveScene().name);
        }  
    }

}
