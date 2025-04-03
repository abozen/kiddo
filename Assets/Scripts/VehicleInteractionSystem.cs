using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Main controller for vehicle interactions
public class VehicleInteractionSystem : MonoBehaviour
{
    [Header("References")]
    public GameObject interactionUI;
    public TextMeshProUGUI interactionText;
    public Animator playerAnimator;
    public CharacterMovement characterMovement;
    public Transform player;

    [Header("Settings")]
    public float enterExitSpeed = 5f;
    public float detectionRadius = 1f;
    public LayerMask vehicleLayer;
    
    [Header("Debug")]
    public bool showDebugGizmos = true;
    
    // Private variables
    private Vehicle currentVehicle;
    private bool isInVehicle = false;
    private bool isTransitioning = false;
    private Vector3 exitPosition;
    private Quaternion exitRotation;
    
    void Update()
    {
        if (isTransitioning)
            return;
            
        if (!isInVehicle)
        {
            // Detect nearby vehicles when not in a vehicle
            DetectNearbyVehicles();
            
            // Handle enter input
            if (currentVehicle != null && Input.GetKeyDown(KeyCode.E))
            {
                EnterVehicle();
            }
        }
        else
        {
            // Update UI text when in vehicle
            UpdateVehicleUI();
            
            // Handle exit input
            if (Input.GetKeyDown(KeyCode.E))
            {
                ExitVehicle();
            }
        }
    }

    public void InteractVehicle()
    {
        if (!isInVehicle)
        {
            // Handle enter input
            if (currentVehicle != null )
            {
                EnterVehicle();
            }
        }
        else
        {
            
                ExitVehicle();
        }
    }
    
    private void DetectNearbyVehicles()
    {
        Collider[] hitColliders = Physics.OverlapSphere(player.position, detectionRadius, vehicleLayer);
        
        if (hitColliders.Length > 0)
        {
            // Get the closest vehicle
            float closestDistance = float.MaxValue;
            Vehicle closestVehicle = null;
            
            foreach (var hitCollider in hitColliders)
            {
                Vehicle vehicle = hitCollider.GetComponent<Vehicle>();
                if (vehicle != null)
                {
                    float distance = Vector3.Distance(player.position, vehicle.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestVehicle = vehicle;
                    }
                }
            }
            
            // Set current vehicle and show UI
            if (closestVehicle != null && closestVehicle != currentVehicle)
            {
                currentVehicle = closestVehicle;
                ShowInteractionUI(true, "Press E to Enter Vehicle");
            }
        }
        else
        {
            // Hide UI if no vehicles nearby
            currentVehicle = null;
            ShowInteractionUI(false);
        }
    }
    
    private void EnterVehicle()
    {
        if (currentVehicle == null || isTransitioning)
            return;
            
        isTransitioning = true;
        
        currentVehicle.SetCollider(true);
        // Disable player controller
        characterMovement.enabled = false;
        
        // Play animation
        if (playerAnimator != null)
            playerAnimator.SetTrigger("EnterVehicle");
            
        // Start transition coroutine
        StartCoroutine(TransitionToVehicle());
    }
    
    private System.Collections.IEnumerator TransitionToVehicle()
    {
        Transform driverSeat = currentVehicle.GetDriverSeat();
        float startTime = Time.time;
        float journeyLength = Vector3.Distance(player.position, driverSeat.position);
        Vector3 startPosition = player.position;
        Quaternion startRotation = player.rotation;
        
        // Move player to driver seat
        while (Vector3.Distance(player.position, driverSeat.position) > 0.01f)
        {
            float distCovered = (Time.time - startTime) * enterExitSpeed;
            float fractionOfJourney = distCovered / journeyLength;
            
            player.position = Vector3.Lerp(startPosition, driverSeat.position, fractionOfJourney);
            player.rotation = Quaternion.Slerp(startRotation, driverSeat.rotation, fractionOfJourney);
            
            yield return null;
        }
        
        // Snap to final position/rotation
        player.position = driverSeat.position;
        player.rotation = driverSeat.rotation;
        
        // Parent player to vehicle
        player.SetParent(driverSeat);
        
        // Enable vehicle controls
        currentVehicle.EnableControl(true);
        
        // Update UI
        ShowInteractionUI(true, "Press E to Exit Vehicle");
        
        isInVehicle = true;
        isTransitioning = false;
    }
    
    private void ExitVehicle()
    {
        if (currentVehicle == null || isTransitioning)
            return;
            
        isTransitioning = true;

        // Store exit position 
        Transform exitPoint = currentVehicle.GetExitPoint();
        exitPosition = exitPoint.position;
        exitRotation = exitPoint.rotation;
        
        
        // Disable vehicle controls
        currentVehicle.EnableControl(false);
        
        // Play animation
        if (playerAnimator != null)
            playerAnimator.SetTrigger("ExitVehicle");
            
        // Start exit coroutine
        StartCoroutine(TransitionFromVehicle());
    }
    
    private System.Collections.IEnumerator TransitionFromVehicle()
    {
        float startTime = Time.time;
        float journeyLength = Vector3.Distance(player.position, exitPosition);
        Vector3 startPosition = player.position;
        Quaternion startRotation = player.rotation;
        
        // Unparent from vehicle
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
        currentVehicle.SetCollider(false);

        
        // Update flags
        isInVehicle = false;
        isTransitioning = false;
        
        // Hide UI until player finds another vehicle
        ShowInteractionUI(false);
    }
    
    private void UpdateVehicleUI()
    {
        if (currentVehicle != null)
        {
            string vehicleInfo = currentVehicle.GetVehicleInfo();
            ShowInteractionUI(true, $"{vehicleInfo}\nPress E to Exit");
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