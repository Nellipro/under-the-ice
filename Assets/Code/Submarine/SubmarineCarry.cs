using UnityEngine;

public class SubmarineCarry : MonoBehaviour
{
    public Rigidbody submarineRb;
    public Rigidbody playerRb;

    private Vector3 lastPosition;
    private Quaternion lastRotation;

    private void Awake()
    {
        //lastPosition = submarineRb.position;
        lastRotation = submarineRb.rotation;
    }

    private void FixedUpdate()
    {
        Vector3 currentPosition = submarineRb.position;
        Quaternion currentRotation = submarineRb.rotation;

        // delta since last physics frame
        Vector3 deltaPosition = currentPosition - lastPosition;
        Quaternion deltaRotation = currentRotation * Quaternion.Inverse(lastRotation);

        // apply the same delta to the player rigidbody
        playerRb.MovePosition(playerRb.position + deltaPosition);
        playerRb.MoveRotation(playerRb.rotation * deltaRotation);

        // store for next frame
        lastPosition = currentPosition;
        lastRotation = currentRotation;
    }
}
