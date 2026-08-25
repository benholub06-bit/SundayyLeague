using UnityEngine;

public class RagdollStanding : MonoBehaviour
{
    [Header("References")]
    public Rigidbody pelvis;
    public Rigidbody leftShin;
    public Rigidbody rightShin;

    [Header("Virtual Feet")]
    public float groundCheckDistance = 0.45f;
    public float supportStrength = 12f;
    public float supportDamping = 2f;

    [Header("Pelvis Balance")]
    public float uprightStrength = 35f;
    public float uprightDamping = 6f;
    public float maxUprightTorque = 60f;

    private Vector3 pelvisLocalUp;

    void Awake()
    {
        pelvisLocalUp =
            Quaternion.Inverse(pelvis.rotation) * Vector3.up;
    }

    void FixedUpdate()
    {
        SupportLeg(leftShin);
        SupportLeg(rightShin);

        KeepPelvisUpright();
    }

    void SupportLeg(Rigidbody shin)
    {
        if (shin == null)
            return;

        Vector3 rayStart = shin.worldCenterOfMass;

        if (Physics.Raycast(
            rayStart,
            Vector3.down,
            out RaycastHit hit,
            groundCheckDistance))
        {
            float compression =
                1f - (hit.distance / groundCheckDistance);

            float downwardSpeed =
                Vector3.Dot(
                    shin.linearVelocity,
                    Vector3.down
                );

            float force =
                compression * supportStrength
                + downwardSpeed * supportDamping;

            force = Mathf.Clamp(force, 0f, supportStrength);

            pelvis.AddForceAtPosition(
                Vector3.up * force,
                hit.point,
                ForceMode.Acceleration
            );
        }
    }

    void KeepPelvisUpright()
    {
        Vector3 currentUp =
            pelvis.rotation * pelvisLocalUp;

        Vector3 correctionAxis =
            Vector3.Cross(currentUp, Vector3.up);

        Vector3 tiltVelocity =
            Vector3.ProjectOnPlane(
                pelvis.angularVelocity,
                Vector3.up
            );

        Vector3 torque =
            correctionAxis * uprightStrength
            - tiltVelocity * uprightDamping;

        torque = Vector3.ClampMagnitude(
            torque,
            maxUprightTorque
        );

        pelvis.AddTorque(
            torque,
            ForceMode.Acceleration
        );
    }
}