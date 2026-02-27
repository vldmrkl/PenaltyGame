using UnityEngine;

public class GoalAndSaveDetector : MonoBehaviour
{
    public MatchController matchController;
    public SwipeShooter shooter;
    public float maxShotTime = 2.5f;      // seconds until we call it a miss
    public float stopSpeed = 0.6f;        // if ball slows below this, it’s basically done
    public float stopGrace = 0.25f;       // how long it must stay slow

    private Rigidbody rb;
    private bool resolved;
    private bool shotInProgress;
    private float shotStartTime;
    private float slowTime;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (!shotInProgress || resolved) return;

        // 1) Timeout -> MISS
        if (Time.time - shotStartTime > maxShotTime)
        {
            ResolveMiss();
            return;
        }

        // 2) Ball basically stopped -> MISS
        if (rb.linearVelocity.magnitude < stopSpeed)
        {
            slowTime += Time.deltaTime;
            if (slowTime >= stopGrace)
            {
                ResolveMiss();
            }
        }
        else
        {
            slowTime = 0f;
        }
    }

    public void NotifyShotFired()
    {
        shotInProgress = true;
        resolved = false;
        slowTime = 0f;
        shotStartTime = Time.time;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (resolved) return;

        if (other.CompareTag("GoalTrigger"))
        {
            resolved = true;
            shotInProgress = false;
            matchController.GoalScored();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (resolved) return;

        if (collision.collider.CompareTag("Goalkeeper"))
        {
            resolved = true;
            shotInProgress = false;
            matchController.ShotSaved();
        }
    }

    private void ResolveMiss()
    {
        if (resolved) return;
        resolved = true;
        shotInProgress = false;
        matchController.MissedShot();
    }
}