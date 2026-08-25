using UnityEngine;

public class RagdollUpright : MonoBehaviour
{
    public Rigidbody pelvis;
    public float strength = 80f;
    public float damping = 12f;

    private Vector3 localUp;

    void Awake()
    {
        if (pelvis == null)
            pelvis = GetComponent<Rigidbody>();

        localUp = Quaternion.Inverse(pelvis.rotation) * Vector3.up;
    }

    void FixedUpdate()
    {
        Vector3 currentUp = pelvis.rotation * localUp;

        Vector3 correctionAxis =
            Vector3.Cross(currentUp, Vector3.up);

        Vector3 tiltVelocity =
            Vector3.ProjectOnPlane(
                pelvis.angularVelocity,
                Vector3.up
            );

        Vector3 torque =
            correctionAxis * strength
            - tiltVelocity * damping;

        pelvis.AddTorque(
            torque,
            ForceMode.Acceleration
        );
    }
}