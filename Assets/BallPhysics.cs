using UnityEngine;

public class BallPhysics : MonoBehaviour
{
    public Rigidbody rb;

    public float maxSpeed = 18f;
    public float maxAngularSpeed = 25f;

    void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity =
                rb.linearVelocity.normalized *
                maxSpeed;
        }

        if (rb.angularVelocity.magnitude > maxAngularSpeed)
        {
            rb.angularVelocity =
                rb.angularVelocity.normalized *
                maxAngularSpeed;
        }
    }
}