using UnityEngine;

public class Skateboard : MonoBehaviour
{
    [Header("Skateboard Settings")]
    public string skateboardName = "Skateboard";
    public Transform standingPoint;
    public Transform exitPoint;

    [Header("Skateboard Components")]
    public SkateboardController skateboardController;
    public Collider skateboardCollider;
    public Collider playerCollider;

    private void Start()
    {
        // Disable skateboard control at start
        if (skateboardController != null)
            skateboardController.enabled = false;
    }

    public Transform GetStandingPoint()
    {
        return standingPoint;
    }

    public Transform GetExitPoint()
    {
        return exitPoint;
    }

    public string GetSkateboardInfo()
    {
        return skateboardName;
    }

    public void EnableControl(bool enable)
    {
        if (skateboardController != null)
            skateboardController.enabled = enable;
    }

    public void SetCollider(bool ignore)
    {
        Physics.IgnoreCollision(skateboardCollider, playerCollider, ignore);
    }
} 