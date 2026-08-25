using System.Collections.Generic;
using UnityEngine;

public class ActiveRagdollController : MonoBehaviour
{
    [Header("Main Body")]
    public Rigidbody pelvis;

    [Header("Bodies To Control")]
    public Rigidbody[] bodyParts;

    [Header("Muscle Settings")]
    public float strength = 250f;
    public float damping = 25f;
    public float maxTorque = 300f;

    [Header("Standing Support")]
    public float standingHeight = 0.9f;
    public float standingSpring = 40f;
    public float standingDamping = 12f;
    public float groundCheckDistance = 1.5f;

    [Header("State")]
    public bool active = true;

    private class BodyTarget
    {
        public Rigidbody body;
        public Transform parent;
        public Quaternion startingLocalRotation;
    }

    private readonly List<BodyTarget> targets = new List<BodyTarget>();

    private Quaternion pelvisStartingRotation;

    void Start()
    {
        if (pelvis != null)
        {
            pelvisStartingRotation = pelvis.rotation;
            pelvis.maxAngularVelocity = 20f;
        }

        foreach (Rigidbody rb in bodyParts)
        {
            if (rb == null)
                continue;

            BodyTarget target = new BodyTarget
            {
                body = rb,
                parent = rb.transform.parent,
                startingLocalRotation = rb.transform.localRotation
            };

            targets.Add(target);

            rb.maxAngularVelocity = 20f;
        }
    }

    void FixedUpdate()
    {
        if (!active)
            return;

        HoldPelvis();
        SupportStandingHeight();

        foreach (BodyTarget target in targets)
        {
            HoldBodyPart(target);
        }
    }

    void HoldPelvis()
    {
        if (pelvis == null)
            return;

        ApplyRotationTorque(
            pelvis,
            pelvisStartingRotation
        );
    }

    void HoldBodyPart(BodyTarget target)
    {
        if (target.body == null || target.parent == null)
            return;

        Quaternion targetWorldRotation =
            target.parent.rotation *
            target.startingLocalRotation;

        ApplyRotationTorque(
            target.body,
            targetWorldRotation
        );
    }

    void SupportStandingHeight()
    {
        if (pelvis == null)
            return;

        if (Physics.Raycast(
            pelvis.worldCenterOfMass,
            Vector3.down,
            out RaycastHit hit,
            groundCheckDistance))
        {
            float heightError =
                standingHeight - hit.distance;

            float verticalVelocity =
                Vector3.Dot(
                    pelvis.linearVelocity,
                    Vector3.up
                );

            float force =
                heightError * standingSpring
                - verticalVelocity * standingDamping;

            force = Mathf.Clamp(
                force,
                -10f,
                20f
            );

            pelvis.AddForce(
                Vector3.up * force,
                ForceMode.Acceleration
            );
        }
    }

    void ApplyRotationTorque(
        Rigidbody body,
        Quaternion targetRotation)
    {
        Quaternion difference =
            targetRotation *
            Quaternion.Inverse(body.rotation);

        difference.ToAngleAxis(
            out float angle,
            out Vector3 axis
        );

        if (angle > 180f)
            angle -= 360f;

        if (axis.sqrMagnitude < 0.001f)
            return;

        axis.Normalize();

        Vector3 torque =
            axis *
            (angle * Mathf.Deg2Rad * strength)
            - body.angularVelocity * damping;

        torque = Vector3.ClampMagnitude(
            torque,
            maxTorque
        );

        body.AddTorque(
            torque,
            ForceMode.Acceleration
        );
    }
}