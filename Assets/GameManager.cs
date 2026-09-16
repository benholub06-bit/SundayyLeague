using UnityEngine;
using System.Collections;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Ball")]
    public Rigidbody ball;
    public Transform ballSpawn;


    [Header("Player 1")]
    public Rigidbody player1;
    public Transform player1Spawn;
    public PlayerMovement player1Movement;
    public PlayerFling player1Fling;


    [Header("Player 2")]
    public Rigidbody player2;
    public Transform player2Spawn;
    public PlayerMovement player2Movement;
    public PlayerFling player2Fling;


    [Header("Score")]
    public int player1Score = 0;
    public int player2Score = 0;

    public TMP_Text player1ScoreText;
    public TMP_Text player2ScoreText;


    [Header("Timer")]
    public float matchTime = 60f;
    public TMP_Text timerText;


    [Header("Match UI")]
    public TMP_Text countdownText;
    public TMP_Text endText;


    [Header("Reset")]
    public float resetDelay = 1.5f;


    float timeRemaining;

    bool resetting = false;
    bool matchRunning = false;


    void Start()
    {
        timeRemaining = matchTime;

        UpdateScoreUI();
        UpdateTimerUI();

        if (endText != null)
            endText.gameObject.SetActive(false);

        StartCoroutine(
            StartCountdown()
        );
    }


    void Update()
    {
        if (!matchRunning)
            return;


        timeRemaining -= Time.deltaTime;


        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;

            UpdateTimerUI();

            EndMatch();

            return;
        }


        UpdateTimerUI();
    }


    IEnumerator StartCountdown()
    {
        SetGameplay(false);


        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(true);

            countdownText.text = "3";
        }

        yield return new WaitForSeconds(1f);


        if (countdownText != null)
            countdownText.text = "2";

        yield return new WaitForSeconds(1f);


        if (countdownText != null)
            countdownText.text = "1";

        yield return new WaitForSeconds(1f);


        if (countdownText != null)
            countdownText.text = "GO!";


        SetGameplay(true);

        matchRunning = true;


        yield return new WaitForSeconds(0.7f);


        if (countdownText != null)
            countdownText.gameObject.SetActive(false);
    }


    public void GoalScored(GoalTrigger goal)
    {
        if (resetting || !matchRunning)
            return;


        if (goal.scoringPlayer == 1)
        {
            player1Score++;
        }
        else if (goal.scoringPlayer == 2)
        {
            player2Score++;
        }


        UpdateScoreUI();


        StartCoroutine(
            ResetRound(goal)
        );
    }


    void EndMatch()
    {
        matchRunning = false;

        StopAllCoroutines();

        SetGameplay(false);


        ball.linearVelocity =
            Vector3.zero;

        ball.angularVelocity =
            Vector3.zero;


        if (endText != null)
        {
            endText.gameObject.SetActive(true);


            if (player1Score > player2Score)
            {
                endText.text =
                    "TIME UP!\n\nBLUE WINS!";
            }
            else if (player2Score > player1Score)
            {
                endText.text =
                    "TIME UP!\n\nRED WINS!";
            }
            else
            {
                endText.text =
                    "TIME UP!\n\nDRAW!";
            }
        }
    }


    void SetGameplay(bool enabled)
    {
        if (player1Movement != null)
            player1Movement.enabled = enabled;

        if (player2Movement != null)
            player2Movement.enabled = enabled;


        if (player1Fling != null)
            player1Fling.enabled = enabled;

        if (player2Fling != null)
            player2Fling.enabled = enabled;


        if (!enabled)
        {
            player1.linearVelocity =
                Vector3.zero;

            player1.angularVelocity =
                Vector3.zero;


            player2.linearVelocity =
                Vector3.zero;

            player2.angularVelocity =
                Vector3.zero;


            ball.linearVelocity =
                Vector3.zero;

            ball.angularVelocity =
                Vector3.zero;
        }
    }


    void UpdateScoreUI()
    {
        if (player1ScoreText != null)
        {
            player1ScoreText.text =
                player1Score.ToString();
        }


        if (player2ScoreText != null)
        {
            player2ScoreText.text =
                player2Score.ToString();
        }
    }


    void UpdateTimerUI()
    {
        if (timerText == null)
            return;


        int totalSeconds =
            Mathf.CeilToInt(timeRemaining);


        int minutes =
            totalSeconds / 60;


        int seconds =
            totalSeconds % 60;


        timerText.text =
            minutes.ToString() +
            ":" +
            seconds.ToString("00");
    }


    IEnumerator ResetRound(GoalTrigger goal)
    {
        resetting = true;


        yield return new WaitForSeconds(
            resetDelay
        );


        ResetBall();


        ResetPlayer(
            player1,
            player1Spawn
        );


        ResetPlayer(
            player2,
            player2Spawn
        );


        goal.ResetGoal();


        resetting = false;
    }


    public void ResetBall()
    {
        ball.linearVelocity =
            Vector3.zero;

        ball.angularVelocity =
            Vector3.zero;


        ball.position =
            ballSpawn.position;

        ball.rotation =
            ballSpawn.rotation;
    }


    public void ResetPlayer(
        Rigidbody player,
        Transform spawn)
    {
        player.linearVelocity =
            Vector3.zero;

        player.angularVelocity =
            Vector3.zero;


        player.position =
            spawn.position;

        player.rotation =
            spawn.rotation;
    }
}