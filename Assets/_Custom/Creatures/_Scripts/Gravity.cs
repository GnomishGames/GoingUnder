using UnityEngine;

public class Gravity : MonoBehaviour
{
    public float gravity = -9.81f; // Gravity strength
    private Vector3 verticalVelocity; // Current vertical velocity
    private CharacterController characterController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        // Apply gravity
        if (characterController.isGrounded)
        {
            verticalVelocity.y = -0.5f; // Small downward force to keep grounded
        }
        else
        {
            verticalVelocity.y += gravity * Time.deltaTime;
        }

        // Apply vertical velocity
        characterController.Move(verticalVelocity * Time.deltaTime);
    }
}
