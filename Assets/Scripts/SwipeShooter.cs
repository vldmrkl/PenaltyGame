using UnityEngine;

public class SwipeShooter : MonoBehaviour
{
    [Header("References")]
    public Rigidbody ballRb;
    public Transform ballSpawn;
    public Transform goalCenter;   // center of goal mouth (e.g. 0,1.2,18)
    public Camera mainCam;
    public GoalkeeperController goalkeeper;
    public GoalAndSaveDetector detector;

    [Header("Goal Size (meters)")]
    public float goalHalfWidth = 3.66f;   // 7.32m / 2
    public float goalHeight = 2.44f;      // crossbar height
    public float aimMargin = 0.15f;       // keep a small margin from posts/bar

    [Header("Power Tuning")]
    public float powerScale = 0.012f;
    public float minPower = 7f;
    public float maxPower = 18f;

    [Header("Flight Feel")]
    public float extraLift = 0.08f; // small lift so shots aren’t perfectly flat

    private Vector2 startPos;
    private float startTime;
    private bool isDragging;
    private bool shotLocked;

    void Start()
    {
        if (!mainCam) mainCam = Camera.main;
        ResetBall();
    }

    void Update()
    {
        if (shotLocked) return;

        // Touch
        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);

            if (t.phase == TouchPhase.Began)
            {
                isDragging = true;
                startPos = t.position;
                startTime = Time.time;
            }
            else if (t.phase == TouchPhase.Ended && isDragging)
            {
                isDragging = false;
                Vector2 endPos = t.position;
                float dt = Mathf.Max(0.05f, Time.time - startTime);
                Vector2 delta = endPos - startPos;

                if (delta.magnitude < 40f) return;

                Shoot(endPos, delta, dt);
            }
        }

        // Editor mouse convenience
        if (Application.isEditor)
        {
            if (Input.GetMouseButtonDown(0))
            {
                isDragging = true;
                startPos = Input.mousePosition;
                startTime = Time.time;
            }
            if (Input.GetMouseButtonUp(0) && isDragging)
            {
                isDragging = false;
                Vector2 endPos = Input.mousePosition;
                float dt = Mathf.Max(0.05f, Time.time - startTime);
                Vector2 delta = endPos - startPos;

                if (delta.magnitude < 40f) return;

                Shoot(endPos, delta, dt);
            }
        }
    }

    private void Shoot(Vector2 endScreenPos, Vector2 delta, float dt)
    {
        // Power from swipe speed
        float speed = delta.magnitude / dt;
        float power = Mathf.Clamp(speed * powerScale, minPower, maxPower);

        // Ray from camera through finger end position
        Ray ray = mainCam.ScreenPointToRay(endScreenPos);

        // Intersect with plane at goalCenter.z (goal mouth plane)
        float planeZ = goalCenter.position.z;
        float t = (planeZ - ray.origin.z) / ray.direction.z;

        // If camera is behind the plane, t should be positive
        if (t <= 0f) return;

        Vector3 hit = ray.origin + ray.direction * t;

        // Clamp hit point inside goal rectangle (with margin)
        float minX = goalCenter.position.x - goalHalfWidth + aimMargin;
        float maxX = goalCenter.position.x + goalHalfWidth - aimMargin;
        float minY = goalCenter.position.y + aimMargin;
        float maxY = goalCenter.position.y + goalHeight - aimMargin;

        hit.x = Mathf.Clamp(hit.x, minX, maxX);
        hit.y = Mathf.Clamp(hit.y, minY, maxY);
        hit.z = planeZ;

        goalkeeper.ReactToShot(hit);

        // Direction from ball to aimed point
        Vector3 dir = (hit - ballRb.position).normalized;

        // Tiny lift so low aims still feel like a real kick
        dir = (dir + Vector3.up * extraLift).normalized;

        ballRb.linearVelocity = Vector3.zero;
        ballRb.angularVelocity = Vector3.zero;

        ballRb.AddForce(dir * power, ForceMode.Impulse);
        detector.NotifyShotFired();

        shotLocked = true;
    }

    public void ResetBall()
    {
        shotLocked = false;
        ballRb.linearVelocity = Vector3.zero;
        ballRb.angularVelocity = Vector3.zero;
        ballRb.transform.position = ballSpawn.position;
        ballRb.transform.rotation = Quaternion.identity;
    }
}