using UnityEngine;

public class SharedCamera : MonoBehaviour
{
    [Header("Player 1")]
    public Transform player1;
    public Transform player1Pelvis;
    public PlayerFling player1Fling;

    [Header("Player 2")]
    public Transform player2;
    public Transform player2Pelvis;
    public PlayerFling player2Fling;

    [Header("Ball")]
    public Transform ball;


    [Header("Camera Position")]
    public float height = 8f;
    public float distance = 12f;


    [Header("Zoom")]
    public float minDistance = 10f;
    public float maxDistance = 18f;
    public float zoomMultiplier = 0.8f;


    [Header("Smoothing")]
    public float moveSmoothness = 5f;
    public float zoomSmoothness = 4f;


    void LateUpdate()
    {
        if (player1 == null ||
            player2 == null ||
            ball == null)
            return;


        // Choose what the camera should track
        Transform p1Target = player1;
        Transform p2Target = player2;


        if (player1Fling != null &&
            player1Fling.IsFlinging &&
            player1Pelvis != null)
        {
            p1Target = player1Pelvis;
        }


        if (player2Fling != null &&
            player2Fling.IsFlinging &&
            player2Pelvis != null)
        {
            p2Target = player2Pelvis;
        }


        // Centre of both players + ball
        Vector3 centre =
            (p1Target.position +
             p2Target.position +
             ball.position) / 3f;


        // Find how spread apart everything is
        float spread =
            Mathf.Max(
                Vector3.Distance(
                    p1Target.position,
                    p2Target.position
                ),

                Vector3.Distance(
                    p1Target.position,
                    ball.position
                ),

                Vector3.Distance(
                    p2Target.position,
                    ball.position
                )
            );


        // Work out zoom
        float targetDistance =
            Mathf.Clamp(
                minDistance +
                spread * zoomMultiplier,

                minDistance,
                maxDistance
            );


        distance =
            Mathf.Lerp(
                distance,
                targetDistance,
                zoomSmoothness *
                Time.deltaTime
            );


        // Fixed sideline position
        Vector3 desiredPosition =
            centre +
            new Vector3(
                -distance,
                height,
                0f
            );


        // Smooth camera movement
        transform.position =
            Vector3.Lerp(
                transform.position,
                desiredPosition,
                moveSmoothness *
                Time.deltaTime
            );


        transform.LookAt(centre);
    }
}