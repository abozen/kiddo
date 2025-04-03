using UnityEngine;

public class SkateboardController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float maxSpeed = 10f;
    public float acceleration = 2f;
    public float turnSpeed = 100f;
    public float jumpForce = 5f;
    
    [Header("Ground Check")]
    public LayerMask groundLayer;
    public float groundCheckDistance = 0.2f;
    public Transform groundCheck;
    
    [Header("References")]
    public Rigidbody rb;
    public Transform skateboardModel;
    
    private float currentSpeed;
    private bool isGrounded;
    private Vector3 moveDirection;
    
    private void Start()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();
    }
    
    private void Update()
    {
        // Check if skateboard is grounded
        isGrounded = Physics.Raycast(groundCheck.position, Vector3.down, groundCheckDistance, groundLayer);
        
        // Get input
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        
        // Calculate movement direction
        moveDirection = transform.forward * verticalInput;
        
        // Handle rotation
        transform.Rotate(Vector3.up * horizontalInput * turnSpeed * Time.deltaTime);
        
        // Handle jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
    
    private void FixedUpdate()
    {
        if (true)
        {
            // Apply movement force
            if (moveDirection != Vector3.zero)
            {
                currentSpeed = Mathf.MoveTowards(currentSpeed, maxSpeed, acceleration * Time.fixedDeltaTime);
                rb.AddForce(moveDirection * currentSpeed, ForceMode.Force);
                Debug.Log("move direction: " + moveDirection + " --  currentSpeed: " + currentSpeed);
            }
            else
            {
                currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, acceleration * Time.fixedDeltaTime);
            }
            
            // Apply ground friction
            if (currentSpeed > 0.1f)
            {
                rb.AddForce(-rb.velocity * 0.1f, ForceMode.Force);
            }
        }
    }
    
    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * groundCheckDistance);
        }
    }
} 