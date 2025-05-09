using UnityEngine;

public class PlayerCarController : MonoBehaviour
{
    [Header("Speed Settings")]
    [SerializeField] private float minSpeed = 5f;
    [SerializeField] private float maxSpeed = 30f;
    [SerializeField] private float currentSpeed = 10f;
    [SerializeField] private float acceleration = 5f;
    [SerializeField] private float deceleration = 8f;
    
    [Header("Steering Settings")]
    [SerializeField] private float horizontalSpeed = 10f;
    [SerializeField] private float minHorizontalPosition = -8f;
    [SerializeField] private float maxHorizontalPosition = 8f;
    
    [Header("Tilt Settings")]
    [SerializeField] private float maxSteeringTilt = 10f;
    [SerializeField] private float maxAccelerationTilt = 5f;
    [SerializeField] private float maxBrakeTilt = 8f;
    [SerializeField] private float tiltSpeed = 3f;
    [SerializeField] private float returnSpeed = 5f;
    [SerializeField] private float tiltResetDuration = 0.5f; // Duration for temporary tilt reset
    
    [Header("References")]
    [SerializeField] private CarGameManager gameManager;
    [SerializeField] private Transform carMesh; // Reference to the car mesh/model transform
    
    [Header("Collision")]
    [SerializeField] private bool stoppedByCollision = false;
    
    // Input flags
    public bool isPressingW = false;
    public bool isPressingS = false;
    public bool isPressingA = false;
    public bool isPressingD = false;
    
    private float targetZRotation = 0f;
    private float targetXRotation = 0f;
    private bool isTiltResetting = false;
    private float tiltResetTimer = 0f;
    
    private void Start()
    {
        // Try to find GameManager if not assigned
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<CarGameManager>();
        }
        
        // If carMesh is not assigned, try to find a suitable child object
        if (carMesh == null)
        {
            // Try to find a child with "body", "mesh", or "model" in its name
            foreach (Transform child in transform)
            {
                string childName = child.name.ToLower();
                if (childName.Contains("mesh") || childName.Contains("body") || childName.Contains("model"))
                {
                    carMesh = child;
                    break;
                }
            }
            
            // If still not found, use the first child as a fallback
            if (carMesh == null && transform.childCount > 0)
            {
                carMesh = transform.GetChild(0);
                Debug.LogWarning("CarMesh not assigned. Using first child as fallback.");
            }
            
            // If no children, use this transform as a last resort
            if (carMesh == null)
            {
                carMesh = transform;
                Debug.LogWarning("CarMesh not assigned and no children found. Using this transform.");
            }
        }
    }
    
    private void Update()
    {
        //if (gameManager != null && !gameManager.IsGameActive())
        //    return;
            
        HandleInput();
        MoveForward();
        UpdateTilt();
        CheckTiltReset();
    }
    
    private void HandleInput()
    {
        // Keyboard input for testing
        isPressingA = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) || isPressingA;
        isPressingD = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) || isPressingD;
        isPressingW = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) || isPressingW;
        isPressingS = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow) || isPressingS;
        
        // Horizontal movement
        float horizontalInput = 0f;
        
        if (isPressingA)
        {
            horizontalInput = -1f;
            targetZRotation = maxSteeringTilt;
        }
        else if (isPressingD)
        {
            horizontalInput = 1f;
            targetZRotation = -maxSteeringTilt;
        }
        else
        {
            targetZRotation = 0f;
        }
        
        // Apply horizontal movement
        if (horizontalInput != 0f)
        {
            MoveHorizontally(horizontalInput);
        }
        
        // Tilt reset on T key press
        if (Input.GetKeyDown(KeyCode.T) && !isTiltResetting)
        {
            isTiltResetting = true;
            tiltResetTimer = 0f;
        }
        
        // Speed control
        if (isPressingW)
        {
            currentSpeed = Mathf.Min(currentSpeed + acceleration * Time.deltaTime, maxSpeed);
            if (!isTiltResetting)
            {
                targetXRotation = -maxAccelerationTilt;
            }
        }
        else if (isPressingS)
        {
            currentSpeed = Mathf.Max(currentSpeed - deceleration * Time.deltaTime, minSpeed);
            if (!isTiltResetting)
            {
                targetXRotation = maxBrakeTilt;
            }
        }
        else
        {
            if (!isTiltResetting)
            {
                targetXRotation = 0f;
            }
        }
    }
    
    private void UpdateTilt()
    {
        if (carMesh == null) return;
        
        // Gradually change rotation to target values
        Quaternion currentRotation = carMesh.localRotation;
        
        // Convert current to euler angles
        Vector3 eulerRotation = currentRotation.eulerAngles;
        
        // Normalize angles to -180 to 180 range
        float currentXRotation = eulerRotation.x;
        if (currentXRotation > 180f) currentXRotation -= 360f;
        
        float currentZRotation = eulerRotation.z;
        if (currentZRotation > 180f) currentZRotation -= 360f;
        
        // Smoothly interpolate towards target rotations
        float newXRotation = Mathf.Lerp(currentXRotation, targetXRotation, Time.deltaTime * (targetXRotation == 0 ? returnSpeed : tiltSpeed));
        float newZRotation = Mathf.Lerp(currentZRotation, targetZRotation, Time.deltaTime * (targetZRotation == 0 ? returnSpeed : tiltSpeed));
        
        // Apply rotation to the car mesh only
        carMesh.localRotation = Quaternion.Euler(newXRotation, eulerRotation.y, newZRotation);
    }
    
    private void CheckTiltReset()
    {
        if (isTiltResetting)
        {
            // Force tilt to default during reset
            targetXRotation = 0f;
            
            // Update timer
            tiltResetTimer += Time.deltaTime;
            
            // End reset after duration expires
            if (tiltResetTimer >= tiltResetDuration)
            {
                isTiltResetting = false;
                
                // Restore proper tilt based on current input
                if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
                {
                    targetXRotation = -maxAccelerationTilt;
                }
                else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
                {
                    targetXRotation = maxBrakeTilt;
                }
            }
        }
    }
    
    private void MoveForward()
    {
        if (!stoppedByCollision)
        {
            transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
        }
    }
    
    private void MoveHorizontally(float direction)
    {
        if (stoppedByCollision) return;
        
        float moveAmount = direction * horizontalSpeed * Time.deltaTime;
        Vector3 newPosition = transform.position + new Vector3(moveAmount, 0f, 0f);
        
        // Clamp horizontal position within road boundaries
        newPosition.x = Mathf.Clamp(newPosition.x, minHorizontalPosition, maxHorizontalPosition);
        
        // Apply new position
        transform.position = newPosition;
    }
    
    // Public methods for UI buttons
    public void SetMoveLeft(bool isPressing)
    {
        isPressingA = isPressing;
    }
    
    public void SetMoveRight(bool isPressing)
    {
        isPressingD = isPressing;
    }
    
    public void ResetSteeringTilt()
    {
        targetZRotation = 0f;
    }
    
    public void SetAccelerate(bool isPressing)
    {
        isPressingW = isPressing;
    }
    
    public void SetBrake(bool isPressing)
    {
        isPressingS = isPressing;
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("TrafficCar"))
        {
            // Stop the car on collision
            stoppedByCollision = true;
            
            // Notify the AI car to stop too via its script
            AITrafficCar trafficCar = collision.gameObject.GetComponent<AITrafficCar>();
            if (trafficCar != null)
            {
                trafficCar.StopAfterCollision();
            }
            
            if (gameManager != null)
            {
                gameManager.GameOver();
            }
        }
    }
    
    // Method to be called externally to stop this car
    public void StopAfterCollision()
    {
        stoppedByCollision = true;
    }
    
    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }
    
    public float GetMaxSpeed()
    {
        return maxSpeed;
    }
} 