using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
    public GameManager gameManager;

    [Header("Scoring")]
    public int scoringPlayer;

    bool goalScored = false;


    void OnTriggerEnter(Collider other)
    {
        if (goalScored)
            return;

        if (!other.CompareTag("Ball"))
            return;


        goalScored = true;

        gameManager.GoalScored(this);
    }


    public void ResetGoal()
    {
        goalScored = false;
    }
}