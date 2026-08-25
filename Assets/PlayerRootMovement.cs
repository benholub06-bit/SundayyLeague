using UnityEngine;

public class PlayerRootMovement : MonoBehaviour
{
    [Header("References")]
    public PlayerControls controls;
    public Rigidbody rootBody;

    [Header("Movement")]
    public float acceleration = 10f;
    public float maxSpeed = 4f;

    void Awake()
    {
        if (controls == null)
            controls = GetComponent<PlayerControls>();

        if (rootBody == null)
            rootBody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (controls == null || rootBody == null)
            return;

        Move();
    }

    void Move()
    {
        Vector2 input = controls.MoveInput;

        Vector3 inputDirection =
            new Vector3(input.x, 0f, input.y);

        if (inputDirection.sqrMagnitude > 1f)
            inputDirection.Normalize();

        Vector3 horizontalVelocity =
            Vector3.ProjectOnPlane(
                rootBody.linearVelocity,
                Vector3.up
            );

        Vector3 desiredVelocity =
            inputDirection * maxSpeed;

        Vector3 velocityDifference =
            desiredVelocity - horizontalVelocity;

        Vector3 force =
            velocityDifference * acceleration;

        rootBody.AddForce(
            force,
            ForceMode.Acceleration
        );
    }
}