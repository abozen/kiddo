using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToyInteractionSystem : MonoBehaviour
{
    [Header("References")]
    public GameObject interactionUI;
    public TextMeshProUGUI interactionText;
    public CharacterMovement characterMovement;
    public Transform player;

    [Header("Settings")]
    public float interactionSpeed = 5f;
    public float detectionRadius = 1f;
    public LayerMask toyLayer;
    
    [Header("Debug")]
    public bool showDebugGizmos = true;
    
    // Private variables
    private Toy currentToy;
    private bool isInteracting = false;
    private bool isTransitioning = false;
    private Vector3 exitPosition;
    private Quaternion exitRotation;
    
    void Update()
    {
        if (isTransitioning)
            return;
            
        if (!isInteracting)
        {
            // Detect nearby toys when not interacting
            DetectNearbyToys();
            
            // Handle enter input
            if (currentToy != null && Input.GetKeyDown(KeyCode.E))
            {
                EnterToy();
            }
        }
    }

    public void InteractToy()
    {
        if (!isInteracting && currentToy != null)
        {
            EnterToy();
        }
    }
    
    private void DetectNearbyToys()
    {
        Collider[] hitColliders = Physics.OverlapSphere(player.position, detectionRadius, toyLayer);
        
        if (hitColliders.Length > 0)
        {
            // Get the closest toy
            float closestDistance = float.MaxValue;
            Toy closestToy = null;
            
            foreach (var hitCollider in hitColliders)
            {
                Toy toy = hitCollider.GetComponent<Toy>();
                if (toy != null)
                {
                    float distance = Vector3.Distance(player.position, toy.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestToy = toy;
                    }
                }
            }
            
            // Set current toy and show UI
            if (closestToy != null && closestToy != currentToy)
            {
                currentToy = closestToy;
                ShowInteractionUI(true, "Press E to Interact with Toy");
            }
        }
        else
        {
            // Hide UI if no toys nearby
            currentToy = null;
            ShowInteractionUI(false);
        }
    }
    
    private void EnterToy()
    {
        if (currentToy == null || isTransitioning)
            return;
            
        isTransitioning = true;
        
        currentToy.SetCollider(true);
        // Disable player controller
        characterMovement.enabled = false;
            
        // Start transition coroutine
        StartCoroutine(TransitionToToy());
    }
    
    private System.Collections.IEnumerator TransitionToToy()
    {
        Transform interactionPoint = currentToy.GetInteractionPoint();
        float startTime = Time.time;
        float journeyLength = Vector3.Distance(player.position, interactionPoint.position);
        Vector3 startPosition = player.position;
        Quaternion startRotation = player.rotation;
        
        // Move player to interaction point
        while (Vector3.Distance(player.position, interactionPoint.position) > 0.01f)
        {
            float distCovered = (Time.time - startTime) * interactionSpeed;
            float fractionOfJourney = distCovered / journeyLength;
            
            player.position = Vector3.Lerp(startPosition, interactionPoint.position, fractionOfJourney);
            player.rotation = Quaternion.Slerp(startRotation, interactionPoint.rotation, fractionOfJourney);
            
            yield return null;
        }
        
        // Snap to final position/rotation
        player.position = interactionPoint.position;
        player.rotation = interactionPoint.rotation;
        
        // Parent player to toy
        player.SetParent(interactionPoint);
        
        // Trigger toy interaction
        currentToy.OnToyInteraction();
        currentToy.SetInteracting(true);
        
        // Update UI
        ShowInteractionUI(false);
        
        isInteracting = true;
        isTransitioning = false;
    }
    
    private void ShowInteractionUI(bool show, string text = "")
    {
        if (interactionUI != null)
        {
            interactionUI.SetActive(show);
            
            if (show && interactionText != null)
                interactionText.text = text;
        }
    }
    
    void OnDrawGizmos()
    {
        if (showDebugGizmos && player != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(player.position, detectionRadius);
        }
    }
} 