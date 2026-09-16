using UnityEngine;

public class KillZone : MonoBehaviour
{
    public GameManager gameManager;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            gameManager.ResetBall();
            return;
        }

        if (other.CompareTag("Player1"))
        {
            gameManager.ResetPlayer(
                gameManager.player1,
                gameManager.player1Spawn
            );

            return;
        }

        if (other.CompareTag("Player2"))
        {
            gameManager.ResetPlayer(
                gameManager.player2,
                gameManager.player2Spawn
            );
        }
    }
}