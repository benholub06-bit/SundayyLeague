using UnityEngine;

public class RagdollHit : MonoBehaviour
{
    public PlayerFling owner;


    void OnCollisionEnter(Collision collision)
    {
        if (owner == null)
            return;


        if (!owner.IsFlinging)
            return;


        PlayerFling otherPlayer =
            collision.collider.GetComponentInParent<PlayerFling>();


        if (otherPlayer == null)
            return;


        if (otherPlayer == owner)
            return;


        Vector3 direction =
            collision.collider.transform.position -
            transform.position;


        direction.y = 0f;


        float speed =
            collision.relativeVelocity.magnitude;


        otherPlayer.HitByFling(
            direction,
            speed
        );
    }
}