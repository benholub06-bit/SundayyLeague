using System.Collections;
using UnityEngine;

public class PlayerFling : MonoBehaviour
{
    [Header("References")]
    public PlayerControls controls;
    public PlayerMovement playerMovement;
    public FloppyMovement floppyMovement;

    public Rigidbody playerRootBody;
    public Collider playerRootCollider;
    public Transform characterRoot;
    public Rigidbody pelvis;
    public Transform cameraTransform;

    [Header("Fling")]
    public float minFlingSpeed = 4f;
    public float maxFlingSpeed = 12f;
    public float upwardSpeed = 3f;
    public float tumbleForce = 3f;
    public float chargeSpeed = 1.5f;
    public float deadZone = 0.3f;

    [Header("Hit")]
    public float hitSpeed = 6f;
    public float hitUpwardSpeed = 1f;

    [Header("Recovery")]
    public float ragdollTime = 1.5f;

    Rigidbody[] ragdollBodies;
    Collider[] ragdollColliders;
    Transform[] bodyTransforms;

    Vector3[] startPositions;
    Quaternion[] startRotations;

    Vector3 characterStartPosition;
    Quaternion characterStartRotation;
    Vector3 characterStartScale;

    Vector3 pelvisOffset;
    Vector3 flingDirection;
    Vector2 lastFlingInput;

    float charge;
    bool charging;
    bool flinging;

    public bool IsFlinging
    {
        get { return flinging; }
    }

    void Start()
    {
        if (controls == null)
            controls = GetComponent<PlayerControls>();

        if (playerMovement == null)
            playerMovement = GetComponent<PlayerMovement>();

        if (playerRootBody == null)
            playerRootBody = GetComponent<Rigidbody>();

        if (playerRootCollider == null)
            playerRootCollider = GetComponent<Collider>();

        ragdollBodies = characterRoot.GetComponentsInChildren<Rigidbody>(true);
        ragdollColliders = characterRoot.GetComponentsInChildren<Collider>(true);

        bodyTransforms = new Transform[ragdollBodies.Length];
        startPositions = new Vector3[ragdollBodies.Length];
        startRotations = new Quaternion[ragdollBodies.Length];

        for (int i = 0; i < ragdollBodies.Length; i++)
        {
            bodyTransforms[i] = ragdollBodies[i].transform;
            startPositions[i] = bodyTransforms[i].localPosition;
            startRotations[i] = bodyTransforms[i].localRotation;
        }

        characterStartPosition = characterRoot.localPosition;
        characterStartRotation = characterRoot.localRotation;
        characterStartScale = characterRoot.localScale;

        pelvisOffset = transform.InverseTransformPoint(pelvis.position);

        SetupHitColliders();
        SetControlledState();
    }

    void Update()
    {
        if (controls == null || flinging)
            return;

        Vector2 input = controls.FlingInput;

        if (input.magnitude > deadZone)
        {
            charging = true;
            lastFlingInput = input;

            charge += chargeSpeed * input.magnitude * Time.deltaTime;
            charge = Mathf.Clamp01(charge);
        }

        if (charging && input.magnitude < deadZone)
        {
            charging = false;
            StartCoroutine(Fling());
        }
    }

    IEnumerator Fling()
    {
        flinging = true;

        Vector3 cameraForward = cameraTransform.forward;
        cameraForward.y = 0f;
        cameraForward.Normalize();

        Vector3 cameraRight = cameraTransform.right;
        cameraRight.y = 0f;
        cameraRight.Normalize();

        flingDirection = -(cameraForward * lastFlingInput.y +
                           cameraRight * lastFlingInput.x);

        flingDirection.y = 0f;
        flingDirection.Normalize();

        float flingSpeed = Mathf.Lerp(minFlingSpeed, maxFlingSpeed, charge);
        Vector3 startingVelocity = playerRootBody.linearVelocity;

        StartRagdoll();

        Vector3 velocity =
            startingVelocity +
            flingDirection * flingSpeed +
            Vector3.up * upwardSpeed;

        foreach (Rigidbody rb in ragdollBodies)
            rb.linearVelocity = velocity;

        Vector3 tumbleAxis =
            Vector3.Cross(Vector3.up, flingDirection).normalized;

        pelvis.AddTorque(tumbleAxis * tumbleForce, ForceMode.Impulse);

        charge = 0f;

        yield return new WaitForSeconds(ragdollTime);

        Recover();
    }

    public void HitByFling(Vector3 direction, float speed)
    {
        if (flinging)
            return;

        flinging = true;

        direction.y = 0f;

        if (direction.sqrMagnitude < 0.01f)
            direction = transform.forward;
        else
            direction.Normalize();

        flingDirection = direction;

        StartRagdoll();

        float finalSpeed = Mathf.Min(speed, hitSpeed);

        Vector3 velocity =
            direction * finalSpeed +
            Vector3.up * hitUpwardSpeed;

        foreach (Rigidbody rb in ragdollBodies)
            rb.linearVelocity = velocity;

        Vector3 tumbleAxis =
            Vector3.Cross(Vector3.up, direction).normalized;

        pelvis.AddTorque(
            tumbleAxis * tumbleForce * 0.6f,
            ForceMode.Impulse
        );

        StartCoroutine(RecoverAfterHit());
    }

    IEnumerator RecoverAfterHit()
    {
        yield return new WaitForSeconds(ragdollTime);
        Recover();
    }

    void StartRagdoll()
    {
        if (playerMovement != null)
            playerMovement.enabled = false;

        if (floppyMovement != null)
            floppyMovement.enabled = false;

        characterRoot.SetParent(null, true);

        playerRootBody.linearVelocity = Vector3.zero;
        playerRootBody.angularVelocity = Vector3.zero;
        playerRootBody.isKinematic = true;

        if (playerRootCollider != null)
            playerRootCollider.enabled = false;

        foreach (Rigidbody rb in ragdollBodies)
            rb.isKinematic = false;

        foreach (Collider col in ragdollColliders)
            col.enabled = true;
    }

    void Recover()
    {
        Vector3 landingPosition = pelvis.position;

        foreach (Rigidbody rb in ragdollBodies)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        foreach (Collider col in ragdollColliders)
            col.enabled = false;

        Quaternion rootRotation =
            Quaternion.LookRotation(flingDirection, Vector3.up);

        playerRootBody.rotation = rootRotation;

        Vector3 rotatedOffset = rootRotation * pelvisOffset;
        playerRootBody.position = landingPosition - rotatedOffset;

        characterRoot.SetParent(transform, true);

        characterRoot.localPosition = characterStartPosition;
        characterRoot.localRotation = characterStartRotation;
        characterRoot.localScale = characterStartScale;

        for (int i = 0; i < bodyTransforms.Length; i++)
        {
            bodyTransforms[i].localPosition = startPositions[i];
            bodyTransforms[i].localRotation = startRotations[i];
        }

        playerRootBody.isKinematic = false;

        if (playerRootCollider != null)
            playerRootCollider.enabled = true;

        if (playerMovement != null)
            playerMovement.enabled = true;

        if (floppyMovement != null)
            floppyMovement.enabled = true;

        flinging = false;
    }

    void SetupHitColliders()
    {
        foreach (Collider col in ragdollColliders)
        {
            RagdollHit hit = col.GetComponent<RagdollHit>();

            if (hit == null)
                hit = col.gameObject.AddComponent<RagdollHit>();

            hit.owner = this;
        }
    }

    void SetControlledState()
    {
        foreach (Rigidbody rb in ragdollBodies)
            rb.isKinematic = true;

        foreach (Collider col in ragdollColliders)
            col.enabled = false;

        playerRootBody.isKinematic = false;

        if (playerRootCollider != null)
            playerRootCollider.enabled = true;
    }
}