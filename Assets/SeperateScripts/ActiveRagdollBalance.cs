using UnityEngine;

public class ActiveRagdollBalance : MonoBehaviour
{
    [Header("Reference")]
    public Rigidbody pelvis;

    [Header("Normal Balance")]
    public float balanceStrength = 100f;
    public float balanceDamping = 15f;

    [Header("Player Balance")]
    public float inputBalanceStrength = 40f;

    [Header("Recovery")]
    public float recoveryStartAngle = 35f;
    public float recoveryTorque = 180f;
    public float recoveryLift = 18f;
    public float maxRecoveryLift = 25f;

    [Header("Ground")]
    public float targetPelvisHeight = 0.9f;
    public float groundCheckDistance = 2f;

    private Vector3 localUp;

    void Awake()
    {
        if (pelvis == null)
            pelvis = GetComponent<Rigidbody>();

        // Store what "up" means for this Blender bone
        localUp =
            Quaternion.Inverse(pelvis.rotation) * Vector3.up;

        pelvis.maxAngularVelocity = 20f;
    }

    void FixedUpdate()
    {
        Balance();
    }

    void Balance()
    {
        Vector3 currentUp =
            pelvis.rotation * localUp;

        float tiltAngle =
            Vector3.Angle(currentUp, Vector3.up);

        // ----------------------------------------
        // PLAYER COUNTERBALANCE
        // ----------------------------------------

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 input =
            new Vector3(horizontal, 0f, vertical);

        if (input.sqrMagnitude > 1f)
            input.Normalize();

        Vector3 inputTorque =
            Vector3.Cross(Vector3.up, input)
            * inputBalanceStrength;

        pelvis.AddTorque(
            inputTorque,
            ForceMode.Acceleration
        );

        // ----------------------------------------
        // AUTOMATIC UPRIGHT TORQUE
        // ----------------------------------------

        Vector3 correctionAxis =
            Vector3.Cross(currentUp, Vector3.up);

        Vector3 tiltVelocity =
            Vector3.ProjectOnPlane(
                pelvis.angularVelocity,
                Vector3.up
            );

        float strength =
            tiltAngle < recoveryStartAngle
            ? balanceStrength
            : recoveryTorque;

        Vector3 correctiveTorque =
            correctionAxis * strength;

        Vector3 dampingTorque =
            -tiltVelocity * balanceDamping;

        pelvis.AddTorque(
            correctiveTorque + dampingTorque,
            ForceMode.Acceleration
        );

        // ----------------------------------------
        // ACTUALLY TRY TO STAND BACK UP
        // ----------------------------------------

        if (tiltAngle > recoveryStartAngle)
        {
            RecoverHeight();
        }
    }

    void RecoverHeight()
    {
        if (Physics.Raycast(
            pelvis.worldCenterOfMass,
            Vector3.down,
            out RaycastHit hit,
            groundCheckDistance))
        {
            float heightError =
                targetPelvisHeight - hit.distance;

            // Only push upward if pelvis is too low
            if (heightError > 0f)
            {
                float lift =
                    heightError * recoveryLift;

                lift = Mathf.Clamp(
                    lift,
                    0f,
                    maxRecoveryLift
                );

                pelvis.AddForce(
                    Vector3.up * lift,
                    ForceMode.Acceleration
                );
            }
        }
    }
}