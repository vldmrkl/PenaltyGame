using UnityEngine;

public class GoalkeeperController : MonoBehaviour
{
    public float moveSpeed = 8f;
    public float reactionDelay = 0.15f;

    private Vector3 startPos;
    private bool reacting;
    private Vector3 targetPos;

    void Start()
    {
        startPos = transform.position;
    }

    public void ReactToShot(Vector3 shotTarget)
    {
        if (reacting) return;

        reacting = true;

        // Predict direction (simple version)
        float xOffset = shotTarget.x - startPos.x;

        float diveAmount = Mathf.Clamp(xOffset, -2.5f, 2.5f);

        targetPos = new Vector3(
            startPos.x + diveAmount,
            startPos.y,
            startPos.z
        );

        Invoke(nameof(StartDive), reactionDelay);
    }

    void StartDive()
    {
        StopAllCoroutines();
        StartCoroutine(Dive());
    }

    System.Collections.IEnumerator Dive()
    {
        while (Vector3.Distance(transform.position, targetPos) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPos,
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }
    }

    public void ResetGK()
    {
        reacting = false;
        transform.position = startPos;
    }
}