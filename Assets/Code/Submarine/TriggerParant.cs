using UnityEngine;

public class TriggerParant : MonoBehaviour
{
    public GameObject player;
    public GameObject Sub;

    void OnTriggerStay(Collider other)
    {
        player.transform.SetParent(Sub.transform, true); 
    }

    void OnTriggerExit(Collider other)
    {
        player.transform.SetParent(null, true);
    }
}
