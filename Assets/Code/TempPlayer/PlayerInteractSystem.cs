using UnityEngine;

interface IInteractible    
{
    public void Interact();
}

public class PlayerInteractSystem : MonoBehaviour
{
    public Transform InteractSource;
    public float InteractRange;

    Vector3 origin;
    Vector3 dir;

    void Start()
    {
        origin = transform.position;
        dir = transform.forward;
    }

    void Update()
    {
       if (Input.GetKey(KeyCode.E)) 
       {
            Ray r = new Ray(InteractSource.position, InteractSource.forward);

            Debug.DrawRay(origin, dir * InteractRange, Color.green, 1f);

            if (Physics.Raycast(r, out RaycastHit hitInfo, InteractRange))
            {
                if (hitInfo.collider.gameObject.TryGetComponent(out IInteractible interactObj))
                {
                    interactObj.Interact();
                }
            }
       }
    }
}
