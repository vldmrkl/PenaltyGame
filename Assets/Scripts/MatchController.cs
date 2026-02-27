using UnityEngine;
using TMPro;

public class MatchController : MonoBehaviour
{
    public SwipeShooter shooter;
    public GoalkeeperController goalkeeper;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI turnText;

    private int playerAScore = 0;
    private int playerBScore = 0;
    private int currentPlayer = 0; // 0 = A, 1 = B
    private int shotsTaken = 0;
    private bool turnActive = true;

    void Start()
    {
        UpdateUI();
    }

    public void StartTurn()
    {
        turnActive = true;
    }

    public void MissedShot()
    {
        if (!turnActive) return;
        turnActive = false;
        EndTurn();
    }

    public void GoalScored()
    {
        if (!turnActive) return;
        turnActive = false;

        if (currentPlayer == 0)
            playerAScore++;
        else
            playerBScore++;

        UpdateUI();
        StartCoroutine(DelayedEndTurn(1.0f));
    }

    private System.Collections.IEnumerator DelayedEndTurn(float delay)
    {
        yield return new WaitForSeconds(delay);
        EndTurn();
    }

    public void ShotSaved()
    {
        if (!turnActive) return;
        turnActive = false;
        StartCoroutine(DelayedEndTurn(1.0f));
    }

    void UpdateUI()
    {
        scoreText.text = playerAScore + " - " + playerBScore;
        turnText.text = currentPlayer == 0 ? "Player A" : "Player B";
    }

    void EndTurn()
    {
        shotsTaken++;
        currentPlayer = 1 - currentPlayer;

        if (shotsTaken >= 10)
        {
            Debug.Log("Game Over");
        }

        goalkeeper.ResetGK();
        shooter.ResetBall();
        StartTurn();
        UpdateUI();
    }
}