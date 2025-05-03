using UnityEngine;

public class AITrafficCar : MonoBehaviour
{
    [Header("Car Settings")]
    [SerializeField] private float carSpeed = 10f;
    [SerializeField] private float brakingDistance = 10f;
    [SerializeField] private float stoppingDistance = 5f;
    [SerializeField] private float despawnDistance = 30f; // Distance behind player to despawn
    
    [Header("Lane Settings")]
    [SerializeField] private float laneWidth = 4f;
    [SerializeField] private float minHorizontalPosition = -8f;
    [SerializeField] private float maxHorizontalPosition = 8f;
    
    [Header("References")]
    [SerializeField] private Transform playerTransform;
    
    private int currentLane = 0;
    private bool isBraking = false;
    
    private void Start()
    {
        // Find the player if not assigned
        if (playerTransform == null)
        {
            PlayerCarController player = FindObjectOfType<PlayerCarController>();
            if (player != null)
            {
                playerTransform = player.transform;
            }
        }
    }
    
    private void Update()
    {
        if (playerTransform == null)
            return;
            
        MoveForward();
        CheckDespawn();
        AvoidCollisions();
    }
    
    private void MoveForward()
    {
        transform.Translate(Vector3.forward * carSpeed * Time.deltaTime);
    }
    
    private void CheckDespawn()
    {
        // Despawn if too far behind the player
        if (transform.position.z < playerTransform.position.z - despawnDistance)
        {
            Destroy(gameObject);
        }
    }
    
    private void AvoidCollisions()
    {
        RaycastHit hit;
        // Cast a ray forward to detect obstacles
        if (Physics.Raycast(transform.position, transform.forward, out hit, brakingDistance))
        {
            // If the obstacle is another car
            if (hit.collider.CompareTag("TrafficCar") || hit.collider.CompareTag("Player"))
            {
                float distance = hit.distance;
                
                // Stop completely if very close
                if (distance <= stoppingDistance)
                {
                    carSpeed = 0f;
                    isBraking = true;
                }
                // Slow down if within braking distance
                else if (distance <= brakingDistance)
                {
                    float brakeIntensity = 1f - ((distance - stoppingDistance) / (brakingDistance - stoppingDistance));
                    
                    // Get the other car's speed (if it's an AI car)
                    AITrafficCar otherCar = hit.collider.GetComponent<AITrafficCar>();
                    float targetSpeed = 0f;
                    
                    if (otherCar != null)
                    {
                        targetSpeed = otherCar.GetSpeed() * 0.9f; // Slightly slower than the car ahead
                    }
                    else if (hit.collider.CompareTag("Player"))
                    {
                        PlayerCarController player = hit.collider.GetComponent<PlayerCarController>();
                        if (player != null)
                        {
                            targetSpeed = player.GetCurrentSpeed() * 0.9f;
                        }
                    }
                    
                    carSpeed = Mathf.Lerp(carSpeed, targetSpeed, brakeIntensity * Time.deltaTime * 5f);
                    isBraking = true;
                }
            }
        }
        else if (isBraking)
        {
            // Gradually return to normal speed when no obstacle ahead
            isBraking = false;
        }
    }
    
    public void SetSpeed(float speed)
    {
        carSpeed = speed;
    }
    
    public float GetSpeed()
    {
        return carSpeed;
    }
    
    public void SetLane(int lane)
    {
        currentLane = lane;
        
        // Calculate target X position for the lane (distribute evenly across the road width)
        float roadWidth = maxHorizontalPosition - minHorizontalPosition;
        int totalLanes = 4; // Keep 4 lanes as specified in requirements
        
        // Position the car in the correct lane
        float laneOffset = roadWidth / totalLanes;
        float xPos = minHorizontalPosition + (lane + 0.5f) * laneOffset;
        
        // Apply position
        Vector3 position = transform.position;
        position.x = xPos;
        transform.position = position;
    }
    
    public int GetLane()
    {
        return currentLane;
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        // Optional: Add collision effects or logic
        if (collision.gameObject.CompareTag("Player"))
        {
            // Maybe add effects here
        }
    }
} 