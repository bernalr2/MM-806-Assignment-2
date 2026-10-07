using UnityEngine;

public class VRLookWalk : MonoBehaviour
{
    public Transform Camera;
    public float toggleAngle = 30.0f;
    public float speed = 3.0f;
    public bool moveForward;

    private CharacterController characterController;

    // Use this for initialization
    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Camera.eulerAngles.x >= toggleAngle && Camera.eulerAngles.x < 90.0f)
        {
            moveForward = true;
        }
        else
        {
            moveForward = false;
        }

        if (moveForward )
        {
            Vector3 forward = Camera.TransformDirection(Vector3.forward);

            characterController.SimpleMove(forward * speed);
        }
    }
}
