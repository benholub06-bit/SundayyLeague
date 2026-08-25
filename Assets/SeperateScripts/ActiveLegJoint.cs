using UnityEngine;

[RequireComponent(typeof(ConfigurableJoint))]
public class ActiveLegJoint : MonoBehaviour
{
    [Header("Drive")]
    public float spring = 500f;
    public float damper = 45f;
    public float maxForce = 1500f;

    private ConfigurableJoint joint;
    private Quaternion startingLocalRotation;

    void Awake()
    {
        joint = GetComponent<ConfigurableJoint>();

        // Remember the standing pose the character starts in
        startingLocalRotation = transform.localRotation;

        joint.rotationDriveMode = RotationDriveMode.Slerp;

        JointDrive drive = joint.slerpDrive;
        drive.positionSpring = spring;
        drive.positionDamper = damper;
        drive.maximumForce = maxForce;

        joint.slerpDrive = drive;
    }

    void FixedUpdate()
    {
        // Drive the bone back toward its original standing orientation
        Quaternion target =
            Quaternion.Inverse(startingLocalRotation);

        joint.targetRotation = target;
    }
}