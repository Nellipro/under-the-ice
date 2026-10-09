using UnityEngine;

public class SodiumPickupInteractable : MonoBehaviour, IInteractible
{
    [SerializeField] private SodiumManeger sodiumManager;

    public void Interact()
    {
        sodiumManager.PickUp();
    }

    public void Pull(Vector3 rayHitPoint)
    {
    }

    public void Release()
    {
    }
}
