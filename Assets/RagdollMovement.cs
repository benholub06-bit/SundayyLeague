using UnityEngine;

public class RagdollMovement : MonoBehaviour
{
    [Header("References")]
    public PlayerControls controls;
    public Rigidbody pelvis;

    [Header("Movement")]
    public float acceleration = 8f;
    public float maxSpeed = 4f;

    [Header("Turning")]
    public float turnStrength = 25f;
    public float turnDamping = 4f;

    void Awake()
    {
        if (controls == null)
            controls = GetComponent<PlayerControls>();
    }

    void FixedUpdate()
    {
        if (controls == null || pelvis == null)
            return;

        Move();
        Turn();
    }

    void Move()
    {
        Vector2 input = controls.MoveInput;

        if (input.sqrMagnitude < 0.01f)
            return;

        Vector3 moveDirection =
            new Vector3(input.x, 0f, input.y);

        if (moveDirection.sqrMagnitude > 1f)
            moveDirection.Normalize();

        Vector3 horizontalVelocity =
            Vector3.ProjectOnPlane(
                pelvis.linearVelocity,
                Vector3.up
            );

        // Stop accelerating once we're near max speed.
        if (horizontalVelocity.magnitude < maxSpeed)
        {
            pelvis.AddForce(
                moveDirection * acceleration,
                ForceMode.Acceleration
            );
        }
    }

    void Turn()
    {
        Vector2 input = controls.MoveInput;

        if (input.sqrMagnitude < 0.05f)
            return;

        Vector3 desiredDirection =
            new Vector3(input.x, 0f, input.y).normalized;

        Vector3 currentForward =
            Vector3.ProjectOnPlane(
                transform.forward,
                Vector3.up
            ).normalized;

        float signedAngle =
            Vector3.SignedAngle(
                currentForward,
                desiredDirection,
                Vector3.up
            );

        float yawVelocity =
            Vector3.Dot(
                pelvis.angularVelocity,
                Vector3.up
            );

        float torque =
            signedAngle * Mathf.Deg2Rad * turnStrength
            - yawVelocity * turnDamping;

        pelvis.AddTorque(
            Vector3.up * torque,
            ForceMode.Acceleration
        );
    }
}