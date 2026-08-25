using UnityEngine;

public class RagdollLegMovement : MonoBehaviour
{
    [Header("Legs")]
    public Rigidbody leftThigh;
    public Rigidbody rightThigh;

    [Header("Walking")]
    public float stepSpeed = 5f;
    public float stepStrength = 25f;
    public float maxStepTorque = 40f;

    private float stepCycle;

    void FixedUpdate()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 input = new Vector3(horizontal, 0f, vertical);

        if (input.magnitude < 0.1f)
            return;

        // Advance walking cycle
        stepCycle += Time.fixedDeltaTime * stepSpeed;

        // -1 to +1 alternating motion
        float step = Mathf.Sin(stepCycle);

        // Legs move opposite each other
        ApplyLegTorque(leftThigh, step);
        ApplyLegTorque(rightThigh, -step);
    }

    void ApplyLegTorque(Rigidbody leg, float direction)
    {
        if (leg == null)
            return;

        // Forward/back swing
        Vector3 torque =
            transform.right * direction * stepStrength;

        torque = Vector3.ClampMagnitude(
            torque,
            maxStepTorque
        );

        leg.AddTorque(
            torque,
            ForceMode.Acceleration
        );

        // Lift the stepping leg slightly
        if (direction > 0f)
        {
            leg.AddForce(
                Vector3.up * 8f,
                ForceMode.Acceleration
            );
        }
    }
}