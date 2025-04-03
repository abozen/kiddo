using UnityEngine;

public class Toy : MonoBehaviour
{
    [Header("References")]
    public Transform interactionPoint;
    public Collider toyCollider;
    public Animator userAnimator;

    private bool isInteracting = false;

    public void SetCollider(bool enabled)
    {
        if (toyCollider != null)
            toyCollider.enabled = enabled;
    }

    public Transform GetInteractionPoint()
    {
        return interactionPoint;
    }

    public void OnToyInteraction()
    {
        // This function will be filled by the user
        userAnimator.SetTrigger("Climb");
    }

    public void ExitToy()
    {
        // This function will be filled by the user
        userAnimator.SetTrigger("ClimbDown");
    }

    public bool IsInteracting()
    {
        return isInteracting;
    }

    public void SetInteracting(bool interacting)
    {
        isInteracting = interacting;
    }
} 