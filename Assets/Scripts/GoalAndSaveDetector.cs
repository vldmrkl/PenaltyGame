using UnityEngine;

public class GoalAndSaveDetector : MonoBehaviour
{
    public SwipeShooter shooter;

    private bool resolved;

    private void OnTriggerEnter(Collider other)
    {
        if (resolved) return;

        if (other.CompareTag("GoalTrigger"))
        {
            resolved = true;
            Debug.Log("GOAL!");
            Invoke(nameof(ResetRound), 1.0f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (resolved) return;

        if (collision.collider.CompareTag("Goalkeeper"))
        {
            resolved = true;
            Debug.Log("SAVED!");
            Invoke(nameof(ResetRound), 1.0f);
        }
    }

    private void ResetRound()
    {
        resolved = false;
        shooter.ResetBall();
    }
}