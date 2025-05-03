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
    
    [Header("References")]
    [SerializeField] private CarGameManager gameManager;
    
    private void Start()
    {
        // Try to find GameManager if not assigned
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<CarGameManager>();
        }
    }
    
    private void Update()
    {
        //if (gameManager != null && !gameManager.IsGameActive())
        //    return;
            
        HandleInput();
        MoveForward();
    }
    
    private void HandleInput()
    {
        // Horizontal movement
        float horizontalInput = 0f;
        
        // Keyboard input for testing
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            horizontalInput = -1f;
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            horizontalInput = 1f;
        }
        
        // Apply horizontal movement
        if (horizontalInput != 0f)
        {
            MoveHorizontally(horizontalInput);
        }
        
        // Speed control
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            Accelerate();
        }
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            Brake();
        }
    }
    
    private void MoveForward()
    {
        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
    }
    
    private void MoveHorizontally(float direction)
    {
        float moveAmount = direction * horizontalSpeed * Time.deltaTime;
        Vector3 newPosition = transform.position + new Vector3(moveAmount, 0f, 0f);
        
        // Clamp horizontal position within road boundaries
        newPosition.x = Mathf.Clamp(newPosition.x, minHorizontalPosition, maxHorizontalPosition);
        
        // Apply new position
        transform.position = newPosition;
    }
    
    // Public methods for UI buttons
    public void MoveLeft()
    {
        MoveHorizontally(-1f);
    }
    
    public void MoveRight()
    {
        MoveHorizontally(1f);
    }
    
    public void Accelerate()
    {
        currentSpeed = Mathf.Min(currentSpeed + acceleration * Time.deltaTime, maxSpeed);
    }
    
    public void Brake()
    {
        currentSpeed = Mathf.Max(currentSpeed - deceleration * Time.deltaTime, minSpeed);
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("TrafficCar") && gameManager != null)
        {
            gameManager.GameOver();
        }
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