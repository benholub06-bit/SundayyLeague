using UnityEngine;

public class PelvisBalance : MonoBehaviour
{
    [Header("Reference")]
    public Rigidbody pelvis;

    [Header("Balance")]
    public float uprightStrength = 45f;
    public float uprightDamping = 8f;
    public float maxTorque = 70f;

    private Vector3 localUp;

    void Awake()
    {
        if (pelvis == null)
            pelvis = GetComponent<Rigidbody>();

        localUp =
            Quaternion.Inverse(pelvis.rotation) * Vector3.up;

        pelvis.maxAngularVelocity = 20f;
    }

    void FixedUpdate()
    {
        Vector3 currentUp =
            pelvis.rotation * localUp;

        Vector3 correctionAxis =
            Vector3.Cross(currentUp, Vector3.up);

        Vector3 tiltAngularVelocity =
            Vector3.ProjectOnPlane(
                pelvis.angularVelocity,
                Vector3.up
            );

        Vector3 torque =
            correctionAxis * uprightStrength
            - tiltAngularVelocity * uprightDamping;

        torque = Vector3.ClampMagnitude(
            torque,
            maxTorque
        );

        pelvis.AddTorque(
            torque,
            ForceMode.Acceleration
        );
    }
}