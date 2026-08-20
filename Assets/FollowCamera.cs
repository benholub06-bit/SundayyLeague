using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform player;
    public Vector3 offset = new Vector3(0f, 5f, -7f);
    public float followSpeed = 8f;

    void LateUpdate()
    {
        Vector3 targetPosition = player.position
                               + player.right * offset.x
                               + Vector3.up * offset.y
                               + player.forward * offset.z;

        transform.position = targetPosition;

        transform.LookAt(player.position + Vector3.up);
    }
}