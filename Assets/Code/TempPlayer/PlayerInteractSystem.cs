using UnityEngine;
using UnityEngine.UI;

interface IInteractible    
{
    public void Interact();
    public void Pull(Vector3 rayHitPoint);
    public void Release();
}

public class PlayerInteractSystem : MonoBehaviour
{
    public Transform InteractSource;
    public float InteractRange;
    public LayerMask InteractLayers;

    Vector3 origin;
    Vector3 dir;
    IInteractible currentInteractable;

    void Start()
    {
        origin = InteractSource.position;
        dir = InteractSource.forward;
    }

    void Update()
    {
       if (Input.GetKeyDown(KeyCode.Mouse0))
       {
            currentInteractable = GetInteractable(out RaycastHit hitInfo);
            if (currentInteractable != null)
            {
                Debug.DrawRay(origin, dir * InteractRange, Color.green, 1f);
                if(currentInteractable !=null)
                {
                    currentInteractable.Interact();
                }
            }
       }

       if (Input.GetKey(KeyCode.Mouse0))
       {
            IInteractible hitInteractable = GetInteractable(out RaycastHit hitInfo);
            if (hitInteractable == currentInteractable)
            {
                Debug.DrawRay(origin, dir * InteractRange, Color.red, 1f);
                if(currentInteractable != null)
                {
                    currentInteractable.Pull(hitInfo.point);
                }
            }
       }

       if (Input.GetKeyUp(KeyCode.Mouse0))
       {
            if (currentInteractable != null)
            {
                currentInteractable.Release();
                if(currentInteractable !=null)
                {
                    currentInteractable = null;
                }
            }
       }
    }

    IInteractible GetInteractable(out RaycastHit hitInfo)
    {
        origin = InteractSource.position;
        dir = InteractSource.forward;

        Ray r = new Ray(origin, dir);
        if (Physics.Raycast(r, out hitInfo, InteractRange, InteractLayers) &&
            hitInfo.collider.gameObject.TryGetComponent(out IInteractible interactObj))
        {
            return interactObj;
        }
        return null;
    }
    
}
