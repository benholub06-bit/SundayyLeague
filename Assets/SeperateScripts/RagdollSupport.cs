using UnityEngine;

public class RagddollSupport : MonoBehaviour
{
    public Rigidbody pelvis;

    public float uprightStrength = 60f;
    public float uprightDamping = 10f;

    public float targetHeight = 0.8f;
    public float heightStrength = 15f;
    public float heightDamping = 8f;

    private Vector3 localUp;

    void Awake()
    {
        if (pelvis == null)
            pelvis = GetComponent<Rigidbody>();

        localUp = Quaternion.Inverse(pelvis.rotation) * Vector3.up;
    }

    void FixedUpdate()
    {
        KeepUpright();
        SupportHeight();
    }

    void KeepUpright()
    {
        Vector3 currentUp = pelvis.rotation * localUp;

        Vector3 correction =
            Vector3.Cross(currentUp, Vector3.up);

        Vector3 tiltVelocity =
            Vector3.ProjectOnPlane(
                pelvis.angularVelocity,
                Vector3.up
            );

        pelvis.AddTorque(
            correction * uprightStrength
            - tiltVelocity * uprightDamping,
            ForceMode.Acceleration
        );
    }

    void SupportHeight()
    {
        if (Physics.Raycast(
            pelvis.worldCenterOfMass,
            Vector3.down,
            out RaycastHit hit,
            1.5f))
        {
            float error = targetHeight - hit.distance;

            float verticalSpeed =
                Vector3.Dot(
                    pelvis.linearVelocity,
                    Vector3.up
                );

            float force =
                error * heightStrength
                - verticalSpeed * heightDamping;

            force = Mathf.Clamp(force, -15f, 15f);

            pelvis.AddForce(
                Vector3.up * force,
                ForceMode.Acceleration
            );
        }
    }
}