using UnityEngine;

public class TempBox : MonoBehaviour, IInteractible
{
    public void Interact()
    {
        Debug.Log(Random.Range(0,100));
    }
}
