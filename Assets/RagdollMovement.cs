using UnityEngine;

public class RagdollMovement : MonoBehaviour
{
    public Rigidbody pelvis;
    public float moveForce = 20f;

    void Awake()
    {
        if (pelvis == null)
            pelvis = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 moveDirection =
            new Vector3(horizontal, 0f, vertical).normalized;

        pelvis.AddForce(
            moveDirection * moveForce,
            ForceMode.Acceleration
        );
    }
}