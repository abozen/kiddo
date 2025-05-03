using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform target; // Player car transform
    [SerializeField] private Vector3 offset = new Vector3(0f, 5f, -7f); // Camera offset from the player
    
    [Header("Follow Settings")]
    [SerializeField] private float smoothSpeed = 5f; // How quickly the camera follows the target
    [SerializeField] private float horizontalFollowFactor = 0.5f; // How much the camera follows horizontal movement (0-1)
    [SerializeField] private float lookAheadDistance = 5f; // Distance to look ahead of the player
    
    [Header("Additional Settings")]
    [SerializeField] private bool followTargetRotation = false; // Whether to follow the target's rotation
    [SerializeField] private float cameraHeight = 5f; // Fixed height of the camera
    [SerializeField] private float fieldOfView = 60f; // Camera's field of view
    
    private Vector3 lastTargetPosition;
    private Vector3 currentVelocity = Vector3.zero;
    private Camera mainCamera;
    
    private void Start()
    {
        mainCamera = GetComponent<Camera>();
        if (mainCamera != null)
        {
            mainCamera.fieldOfView = fieldOfView;
        }
        
        // If target not assigned, try to find the player car
        if (target == null)
        {
            PlayerCarController player = FindObjectOfType<PlayerCarController>();
            if (player != null)
            {
                target = player.transform;
            }
            else
            {
                Debug.LogError("Camera Follow Script: No target assigned and player not found!");
            }
        }
        
        if (target != null)
        {
            lastTargetPosition = target.position;
        }
    }
    
    private void LateUpdate()
    {
        if (target == null)
            return;
            
        // Calculate the desired position
        Vector3 desiredPosition = CalculateDesiredPosition();
        
        // Smoothly interpolate between current position and desired position
        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref currentVelocity, 1f / smoothSpeed);
        transform.position = smoothedPosition;
        
        // Handle camera rotation
        if (followTargetRotation)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, Time.deltaTime * smoothSpeed);
        }
        else
        {
            transform.LookAt(target.position + target.forward * lookAheadDistance);
        }
        
        // Update last target position
        lastTargetPosition = target.position;
    }
    
    private Vector3 CalculateDesiredPosition()
    {
        // Calculate direction and speed of target's movement
        Vector3 targetMovement = (target.position - lastTargetPosition) / Time.deltaTime;
        float horizontalSpeed = Mathf.Abs(targetMovement.x);
        
        // Calculate horizontal offset based on target's horizontal movement
        float dynamicHorizontalOffset = -target.position.x * horizontalFollowFactor;
        
        // Calculate desired position
        Vector3 desiredPosition = new Vector3(
            target.position.x + dynamicHorizontalOffset, 
            target.position.y + cameraHeight, 
            target.position.z + offset.z);
            
        return desiredPosition;
    }
    
    // Method to set a new target (useful for changing cars or respawning)
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        if (target != null)
        {
            lastTargetPosition = target.position;
        }
    }
    
    // Method to smoothly change camera settings during gameplay
    public void SetCameraSettings(float newHeight, float newDistance, float newFOV, float newSmoothSpeed)
    {
        cameraHeight = newHeight;
        offset.z = -newDistance;
        smoothSpeed = newSmoothSpeed;
        
        if (mainCamera != null)
        {
            mainCamera.fieldOfView = newFOV;
        }
    }
} 