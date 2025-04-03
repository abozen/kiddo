using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkateboardInteractionSystem : MonoBehaviour
{
    [Header("References")]
    public GameObject interactionUI;
    public TextMeshProUGUI interactionText;
    public Animator playerAnimator;
    public CharacterMovement characterMovement;
    public Transform player;
    public SkaterAnimator skaterAnimator;

    [Header("Settings")]
    public float enterExitSpeed = 5f;
    public float detectionRadius = 1f;
    public LayerMask skateboardLayer;
    
    [Header("Debug")]
    public bool showDebugGizmos = true;
    
    // Private variables
    private Skateboard currentSkateboard;
    private bool isOnSkateboard = false;
    private bool isTransitioning = false;
    private Vector3 exitPosition;
    private Quaternion exitRotation;

    
    void Update()
    {
        if (isTransitioning)
            return;
            
        if (!isOnSkateboard)
        {
            // Detect nearby skateboards when not on one
            DetectNearbySkateboards();
            
            // Handle enter input
            if (currentSkateboard != null && Input.GetKeyDown(KeyCode.E))
            {
                EnterSkateboard();
            }
        }
        else
        {
            // Update UI text when on skateboard
            UpdateSkateboardUI();
            
            // Handle exit input
            if (Input.GetKeyDown(KeyCode.E))
            {
                ExitSkateboard();
            }
        }
    }

    public void InteractSkateboard()
    {
        if (!isOnSkateboard)
        {
            // Handle enter input
            if (currentSkateboard != null)
            {
                EnterSkateboard();
                skaterAnimator.EnterSkate();
            }
        }
        else
        {
            ExitSkateboard();
            skaterAnimator.ExitSkate();

        }
    }
    
    private void DetectNearbySkateboards()
    {
        Collider[] hitColliders = Physics.OverlapSphere(player.position, detectionRadius, skateboardLayer);
        
        if (hitColliders.Length > 0)
        {
            // Get the closest skateboard
            float closestDistance = float.MaxValue;
            Skateboard closestSkateboard = null;
            
            foreach (var hitCollider in hitColliders)
            {
                Skateboard skateboard = hitCollider.GetComponent<Skateboard>();
                if (skateboard != null)
                {
                    float distance = Vector3.Distance(player.position, skateboard.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestSkateboard = skateboard;
                    }
                }
            }
            
            // Set current skateboard and show UI
            if (closestSkateboard != null && closestSkateboard != currentSkateboard)
            {
                currentSkateboard = closestSkateboard;
                ShowInteractionUI(true, "Press E to Ride Skateboard");
            }
        }
        else
        {
            // Hide UI if no skateboards nearby
            currentSkateboard = null;
            ShowInteractionUI(false);
        }
    }
    
    private void EnterSkateboard()
    {
        if (currentSkateboard == null || isTransitioning)
            return;
            
        isTransitioning = true;
        
        currentSkateboard.SetCollider(true);
        // Disable player controller
        characterMovement.enabled = false;
        
        // Play animation
        if (playerAnimator != null)
            playerAnimator.SetTrigger("EnterSkateboard");
            
        // Start transition coroutine
        StartCoroutine(TransitionToSkateboard());
    }
    
    private System.Collections.IEnumerator TransitionToSkateboard()
    {
        Transform standingPoint = currentSkateboard.GetStandingPoint();
        float startTime = Time.time;
        float journeyLength = Vector3.Distance(player.position, standingPoint.position);
        Vector3 startPosition = player.position;
        Quaternion startRotation = player.rotation;
        
        // Move player to standing point
        while (Vector3.Distance(player.position, standingPoint.position) > 0.01f)
        {
            float distCovered = (Time.time - startTime) * enterExitSpeed;
            float fractionOfJourney = distCovered / journeyLength;
            
            player.position = Vector3.Lerp(startPosition, standingPoint.position, fractionOfJourney);
            player.rotation = Quaternion.Slerp(startRotation, standingPoint.rotation, fractionOfJourney);
            
            yield return null;
        }
        
        // Snap to final position/rotation
        player.position = standingPoint.position;
        player.rotation = standingPoint.rotation;
        
        // Parent player to skateboard
        player.SetParent(standingPoint);
        
        // Enable skateboard controls
        currentSkateboard.EnableControl(true);
        
        // Update UI
        ShowInteractionUI(true, "Press E to Exit Skateboard");
        
        isOnSkateboard = true;
        isTransitioning = false;
    }
    
    private void ExitSkateboard()
    {
        if (currentSkateboard == null || isTransitioning)
            return;
            
        isTransitioning = true;

        // Store exit position 
        Transform exitPoint = currentSkateboard.GetExitPoint();
        exitPosition = exitPoint.position;
        exitRotation = exitPoint.rotation;
        
        // Disable skateboard controls
        currentSkateboard.EnableControl(false);
        
        // Play animation
        if (playerAnimator != null)
            playerAnimator.SetTrigger("ExitSkateboard");
            
        // Start exit coroutine
        StartCoroutine(TransitionFromSkateboard());
    }
    
    private System.Collections.IEnumerator TransitionFromSkateboard()
    {
        float startTime = Time.time;
        float journeyLength = Vector3.Distance(player.position, exitPosition);
        Vector3 startPosition = player.position;
        Quaternion startRotation = player.rotation;
        
        // Unparent from skateboard
        player.SetParent(null);
        
        // Move player to exit position
        while (Vector3.Distance(player.position, exitPosition) > 0.01f)
        {
            float distCovered = (Time.time - startTime) * enterExitSpeed;
            float fractionOfJourney = distCovered / journeyLength;
            
            player.position = Vector3.Lerp(startPosition, exitPosition, fractionOfJourney);
            player.rotation = Quaternion.Slerp(startRotation, exitRotation, fractionOfJourney);
            
            yield return null;
        }
        
        // Snap to final position
        player.position = exitPosition;
        player.rotation = exitRotation;
        
        // Re-enable player controller
        characterMovement.enabled = true;
        currentSkateboard.SetCollider(false);
        
        // Update flags
        isOnSkateboard = false;
        isTransitioning = false;
        
        // Hide UI until player finds another skateboard
        ShowInteractionUI(false);
    }
    
    private void UpdateSkateboardUI()
    {
        if (currentSkateboard != null)
        {
            string skateboardInfo = currentSkateboard.GetSkateboardInfo();
            ShowInteractionUI(true, $"{skateboardInfo}\nPress E to Exit");
        }
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