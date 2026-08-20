using UnityEngine;

public class ActiveRagdollBalance : MonoBehaviour
{
    public Rigidbody pelvis;

    [Header("Rotation")]
    public float rotationStrength = 500f;
    public float rotationDamping = 40f;

    [Header("Standing")]
    public float targetHeight = 1.0f;
    public float standStrength = 200f;
    public float standDamping = 20f;
    public float raycastDistance = 2f;

    private Quaternion targetRotation;

    void Start()
    {
        if (pelvis == null)
            pelvis = GetComponent<Rigidbody>();

        targetRotation = pelvis.rotation;
        pelvis.maxAngularVelocity = 20f;
    }

    void FixedUpdate()
    {
        KeepUpright();
        MaintainHeight();
    }

    void KeepUpright()
    {
        Quaternion rotationDifference =
            targetRotation * Quaternion.Inverse(pelvis.rotation);

        rotationDifference.ToAngleAxis(out float angle, out Vector3 axis);

        if (angle > 180f)
            angle -= 360f;

        Vector3 torque =
            axis * (angle * Mathf.Deg2Rad * rotationStrength)
            - pelvis.angularVelocity * rotationDamping;

        pelvis.AddTorque(torque, ForceMode.Acceleration);
    }

    void MaintainHeight()
    {
        if (Physics.Raycast(
            pelvis.position,
            Vector3.down,
            out RaycastHit hit,
            raycastDistance))
        {
            float heightError = targetHeight - hit.distance;

            float verticalVelocity =
                Vector3.Dot(pelvis.linearVelocity, Vector3.up);

            float liftForce =
                (heightError * standStrength)
                - (verticalVelocity * standDamping);

            pelvis.AddForce(
                Vector3.up * liftForce,
                ForceMode.Acceleration);
        }
    }
}