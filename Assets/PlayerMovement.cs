using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    public PlayerControls controls;
    public Rigidbody body;
    public Transform cameraTransform;

    [Header("Movement")]
    public float maxSpeed = 4.5f;
    public float acceleration = 14f;
    public float deceleration = 18f;
    public float deadZone = 0.15f;

    [Header("Turning")]
    public float turnSpeed = 540f;

    void Awake()
    {
        if (controls == null)
            controls = GetComponent<PlayerControls>();

        if (body == null)
            body = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (controls == null || body == null || cameraTransform == null)
            return;

        HandleMovement();
    }

    void HandleMovement()
    {
        Vector2 input = controls.MoveInput;

        // Ignore tiny stick movements
        if (input.magnitude < deadZone)
        {
            SlowDown();
            return;
        }

        float inputAmount = Mathf.Clamp01(input.magnitude);

        // Get camera forward direction,
        // but remove vertical tilt.
        Vector3 cameraForward = cameraTransform.forward;
        cameraForward.y = 0f;
        cameraForward.Normalize();

        Vector3 cameraRight = cameraTransform.right;
        cameraRight.y = 0f;
        cameraRight.Normalize();

        // Convert stick input into a direction
        // relative to the camera.
        Vector3 moveDirection =
            cameraForward * input.y +
            cameraRight * input.x;

        if (moveDirection.sqrMagnitude > 0.001f)
            moveDirection.Normalize();

        // -------------------------
        // MOVEMENT
        // -------------------------

        Vector3 desiredVelocity =
            moveDirection * maxSpeed * inputAmount;

        Vector3 currentHorizontalVelocity =
            new Vector3(
                body.linearVelocity.x,
                0f,
                body.linearVelocity.z
            );

        Vector3 newHorizontalVelocity =
            Vector3.MoveTowards(
                currentHorizontalVelocity,
                desiredVelocity,
                acceleration * Time.fixedDeltaTime
            );

        body.linearVelocity = new Vector3(
            newHorizontalVelocity.x,
            body.linearVelocity.y,
            newHorizontalVelocity.z
        );

        // -------------------------
        // FACING DIRECTION
        // -------------------------

        if (moveDirection.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(
                    moveDirection,
                    Vector3.up
                );

            Quaternion newRotation =
                Quaternion.RotateTowards(
                    body.rotation,
                    targetRotation,
                    turnSpeed * Time.fixedDeltaTime
                );

            body.MoveRotation(newRotation);
        }
    }

    void SlowDown()
    {
        Vector3 currentHorizontalVelocity =
            new Vector3(
                body.linearVelocity.x,
                0f,
                body.linearVelocity.z
            );

        Vector3 newHorizontalVelocity =
            Vector3.MoveTowards(
                currentHorizontalVelocity,
                Vector3.zero,
                deceleration * Time.fixedDeltaTime
            );

        body.linearVelocity = new Vector3(
            newHorizontalVelocity.x,
            body.linearVelocity.y,
            newHorizontalVelocity.z
        );
    }
}