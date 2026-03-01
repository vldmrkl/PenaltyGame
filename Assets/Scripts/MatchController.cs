using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MatchController : MonoBehaviour
{
    public SwipeShooter shooter;
    public GoalkeeperController goalkeeper;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI turnText;

    public PenaltyDotsUI dotsUI;

    private int shotsPerPlayer = 5;
    private int aShotsTaken = 0;
    private int bShotsTaken = 0;

    private int playerAScore = 0;
    private int playerBScore = 0;
    private int currentPlayer = 0; // 0 = A, 1 = B
    private int shotsTaken = 0;
    private bool turnActive = true;
    private readonly List<ShotResult> aResults = new();
    private readonly List<ShotResult> bResults = new();

    


    void Start()
    {
        UpdateUI();
        dotsUI.Init();
        dotsUI.SetScore(playerAScore, playerBScore);
    }

    public void StartTurn()
    {
        turnActive = true;
    }

    public void MissedShot()
    {
        if (!turnActive) return;
        turnActive = false;

        RegisterMissForCurrentPlayer();
        StartCoroutine(DelayedEndTurn(1.0f));
    }

    private void RegisterMissForCurrentPlayer()
    {
        if (currentPlayer == 0)
        {
            aResults.Add(ShotResult.Missed);
            dotsUI.SetShotResult(0, aShotsTaken, ShotResult.Missed);
            aShotsTaken++;
        }
        else
        {
            bResults.Add(ShotResult.Missed);
            dotsUI.SetShotResult(1, bShotsTaken, ShotResult.Missed);
            bShotsTaken++;
        }

        CheckGameOver();
        dotsUI.SetScore(playerAScore, playerBScore);
    }

    private void CheckGameOver()
    {
        if (aShotsTaken >= shotsPerPlayer && bShotsTaken >= shotsPerPlayer)
        {
            Debug.Log("Game Over!");
            // Later: show results UI
        }
    }

    public void GoalScored()
    {
        if (!turnActive) return;
        turnActive = false;

        if (currentPlayer == 0)
        {
            playerAScore++;
            aResults.Add(ShotResult.Scored);
            dotsUI.SetShotResult(0, aShotsTaken, ShotResult.Scored);
            aShotsTaken++;
        }
        else
        {
            playerBScore++;
            bResults.Add(ShotResult.Scored);
            dotsUI.SetShotResult(1, bShotsTaken, ShotResult.Scored);
            bShotsTaken++;
        }

        CheckGameOver();

        dotsUI.SetScore(playerAScore, playerBScore);
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

        RegisterMissForCurrentPlayer();
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