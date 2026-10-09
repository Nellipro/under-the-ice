using UnityEngine;

public class SodiumDropPointInteractable : MonoBehaviour, IInteractible
{
    [SerializeField] private SodiumManeger sodiumManager;

    public void Interact()
    {
        sodiumManager.Place();
    }

    public void Pull(Vector3 rayHitPoint)
    {
    }

    public void Release()
    {
    }
}
