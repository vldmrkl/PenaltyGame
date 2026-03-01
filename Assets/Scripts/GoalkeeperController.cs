using UnityEngine;

public class GoalkeeperController : MonoBehaviour
{
    [Header("Refs")]
    public Animator animator;      // assign GK_Model Animator
    public Transform modelRoot;    // assign GK_Model transform

    [Header("Tuning")]
    public float reactionDelay = 0.12f;

    private Vector3 modelStartLocalPos;
    private Quaternion modelStartLocalRot;

    private bool isDiving;
    private int pendingDiveDir; // -1 = left, +1 = right

    void Start()
    {
        if (!animator)
        {
            Debug.LogError("GoalkeeperController: Animator not assigned.");
            enabled = false;
            return;
        }

        if (!modelRoot) modelRoot = animator.transform;

        modelStartLocalPos = modelRoot.localPosition;
        modelStartLocalRot = modelRoot.localRotation;

        animator.applyRootMotion = false;
    }

    public void ReactToShot(Vector3 shotTarget)
    {
        if (isDiving) return;
        isDiving = true;

        // decide direction now, execute after delay
        float xOffset = shotTarget.x - transform.position.x;
        pendingDiveDir = (xOffset < 0f) ? -1 : 1;

        // allow Y root motion during dive (so he reaches ground)
        animator.applyRootMotion = true;

        CancelInvoke(nameof(PerformDive));
        Invoke(nameof(PerformDive), reactionDelay);
    }

    private void PerformDive()
    {
        if (pendingDiveDir < 0) animator.SetTrigger("DiveLeft");
        else animator.SetTrigger("DiveRight");
    }

    public void ResetGK()
    {
        CancelInvoke(nameof(PerformDive));

        // stop root motion affecting future frames
        animator.applyRootMotion = false;

        // reset animator to default (Idle)
        animator.Rebind();
        animator.Update(0f);

        // reset model local transform to prevent drift/sinking
        modelRoot.localPosition = modelStartLocalPos;
        modelRoot.localRotation = modelStartLocalRot;

        isDiving = false;
        pendingDiveDir = 0;
    }
}