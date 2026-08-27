using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("References")]
    public Transform target;
    public PlayerControls controls;

    [Header("Camera")]
    public float distance = 6f;
    public float height = 2.5f;

    [Header("Rotation")]
    public float horizontalSensitivity = 120f;
    public float verticalSensitivity = 90f;

    public float minPitch = -20f;
    public float maxPitch = 60f;

    [Header("Smoothing")]
    public float followSmoothness = 12f;

    private float yaw;
    private float pitch = 15f;

    void Start()
    {
        if (target != null)
            yaw = target.eulerAngles.y;
    }

    void LateUpdate()
    {
        if (target == null || controls == null)
            return;

        HandleLook();
        FollowTarget();
    }

    void HandleLook()
    {
        Vector2 look = controls.LookInput;

        yaw += look.x
            * horizontalSensitivity
            * Time.deltaTime;

        pitch -= look.y
            * verticalSensitivity
            * Time.deltaTime;

        pitch = Mathf.Clamp(
            pitch,
            minPitch,
            maxPitch
        );
    }

    void FollowTarget()
    {
        Quaternion rotation =
            Quaternion.Euler(
                pitch,
                yaw,
                0f
            );

        Vector3 targetPoint =
            target.position +
            Vector3.up * height;

        Vector3 desiredPosition =
            targetPoint -
            rotation * Vector3.forward * distance;

        transform.position =
            Vector3.Lerp(
                transform.position,
                desiredPosition,
                followSmoothness * Time.deltaTime
            );

        transform.LookAt(targetPoint);
    }
}