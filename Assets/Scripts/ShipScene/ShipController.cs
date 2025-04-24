using UnityEngine;

public class ShipController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float maxSpeed = 10f;  // Maximum speed at highest gear
    [SerializeField] private float rotationSpeed = 80f;
    [SerializeField] private float accelerationTime = 1f;
    [SerializeField] private float decelerationTime = 0.7f;

    [Header("Speed Levels")]
    [SerializeField] private int maxSpeedLevel = 3;  // Total number of speed levels
    [SerializeField] private KeyCode increaseSpeedKey = KeyCode.W;  // Speed up button
    [SerializeField] private KeyCode decreaseSpeedKey = KeyCode.S;  // Slow down button
    [SerializeField] private KeyCode turnLeftKey = KeyCode.A;  // Turn left button
    [SerializeField] private KeyCode turnRightKey = KeyCode.D;  // Turn right button
    
    [Header("Ship Feel")]
    [SerializeField] private float tiltAmount = 5f;  // Slight tilt when turning
    [SerializeField] private float bobbingAmount = 0.1f;  // Ship bobbing on water
    [SerializeField] private float bobbingSpeed = 1f;

    // Movement variables
    private int currentSpeedLevel = 0;
    private float currentSpeed;
    private float targetSpeed;
    private float currentRotation;
    private Vector3 startPosition;
    private Rigidbody rb;
    
    // Input cooldown to prevent rapid button presses
    private float speedInputCooldown = 0.2f;
    private float speedInputTimer = 0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        // Simple physics setup
        rb.useGravity = false;
        rb.drag = 1f;
        rb.angularDrag = 3f;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        startPosition = transform.position;
    }

    private void Update()
    {
        HandleSpeedInput();
        HandleRotationInput();
        UpdateShipMovement();
        ApplyBobbing();
    }
    
    private void HandleSpeedInput()
    {
        // Manage input cooldown timer
        if (speedInputTimer > 0)
        {
            speedInputTimer -= Time.deltaTime;
        }
        
        // Change speed level with cooldown to prevent accidental multiple presses
        if (speedInputTimer <= 0)
        {
            if (Input.GetKeyDown(increaseSpeedKey) && currentSpeedLevel < maxSpeedLevel)
            {
                currentSpeedLevel++;
                speedInputTimer = speedInputCooldown;
                Debug.Log("Speed Level: " + currentSpeedLevel);
            }
            else if (Input.GetKeyDown(decreaseSpeedKey) && currentSpeedLevel > 0)
            {
                currentSpeedLevel--;
                speedInputTimer = speedInputCooldown;
                Debug.Log("Speed Level: " + currentSpeedLevel);
            }
        }
        
        // Calculate target speed based on current speed level
        targetSpeed = (float)currentSpeedLevel / maxSpeedLevel * maxSpeed;
        
        // Smoothly adjust current speed toward target speed
        float accelerationRate = (targetSpeed > currentSpeed) ? accelerationTime : decelerationTime;
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime / accelerationRate);
    }
    
    private void HandleRotationInput()
    {
        // Handle rotation with left/right buttons
        float rotationInput = 0;
        
        if (Input.GetKey(turnLeftKey))
        {
            rotationInput = -1f;
        }
        else if (Input.GetKey(turnRightKey))
        {
            rotationInput = 1f;
        }
        
        // Set current rotation based on input
        currentRotation = rotationInput * rotationSpeed;
    }
    
    private void UpdateShipMovement()
    {
        // Calculate ship tilt for visual effect
        float targetTilt = -currentRotation * tiltAmount / rotationSpeed;
        Quaternion currentRotationQuat = transform.rotation;
        Vector3 currentEuler = currentRotationQuat.eulerAngles;
        
        // Smooth Z axis tilt only
        float smoothedZ = Mathf.LerpAngle(currentEuler.z, targetTilt, Time.deltaTime * 5f);
        
        // Keep X at 0, preserve Y rotation, apply smoothed Z tilt
        transform.rotation = Quaternion.Euler(0, currentEuler.y, smoothedZ);
    }

    private void FixedUpdate()
    {
        // Move ship forward based on current speed
        rb.velocity = transform.forward * currentSpeed;
        
        // Apply rotation
        transform.Rotate(0, currentRotation * Time.fixedDeltaTime, 0);
    }

    private void ApplyBobbing()
    {
        // Simple sine wave bobbing to simulate being on water
        if (bobbingAmount > 0)
        {
            float bobbingY = startPosition.y + Mathf.Sin(Time.time * bobbingSpeed) * bobbingAmount;
            Vector3 newPos = transform.position;
            newPos.y = bobbingY;
            transform.position = newPos;
        }
    }
    
    // Optional: Add UI text to show current speed level
    public int GetCurrentSpeedLevel()
    {
        return currentSpeedLevel;
    }
}