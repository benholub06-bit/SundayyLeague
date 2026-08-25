using UnityEngine;

public class RagdollWalking : MonoBehaviour
{
    [Header("References")]
    public Rigidbody pelvis;
    public Rigidbody leftFoot;
    public Rigidbody rightFoot;

    [Header("Walking")]
    public float stepFrequency = 2.5f;
    public float liftForce = 12f;
    public float forwardForce = 8f;

    private float stepTimer;
    private bool leftStep = true;

    void FixedUpdate()
    {
        // Get WASD input
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 input = new Vector3(horizontal, 0f, vertical);

        // Don't step if we're not moving
        if (input.magnitude < 0.1f)
        {
            stepTimer = 0f;
            return;
        }

        Vector3 moveDirection = input.normalized;

        // Count towards next step
        stepTimer += Time.fixedDeltaTime;

        if (stepTimer >= 1f / stepFrequency)
        {
            stepTimer = 0f;
            leftStep = !leftStep;
        }

        // Choose which foot is currently stepping
        Rigidbody steppingFoot = leftStep ? leftFoot : rightFoot;

        if (steppingFoot == null)
            return;

        // Lift foot
        steppingFoot.AddForce(
            Vector3.up * liftForce,
            ForceMode.Acceleration
        );

        // Push foot in movement direction
        steppingFoot.AddForce(
            moveDirection * forwardForce,
            ForceMode.Acceleration
        );
    }
}