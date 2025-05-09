using UnityEngine;

public class AITrafficCar : MonoBehaviour
{
    [Header("Car Settings")]
    [SerializeField] private float carSpeed = 10f;
    [SerializeField] private float brakingDistance = 10f;
    [SerializeField] private float stoppingDistance = 5f;
    [SerializeField] private float despawnDistance = 30f; // Distance behind player to despawn
    
    [Header("References")]
    [SerializeField] private Transform playerTransform;
    
    private bool isBraking = false;
    private bool stoppedByCollision = false;
    
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
        if (!stoppedByCollision)
        {
            transform.Translate(Vector3.forward * carSpeed * Time.deltaTime);
        }
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
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Stop the car on collision
            stoppedByCollision = true;
            carSpeed = 0f;
            
            // Stop the player car too
            PlayerCarController playerCar = collision.gameObject.GetComponent<PlayerCarController>();
            if (playerCar != null)
            {
                playerCar.StopAfterCollision();
            }
        }
    }
    
    // Method to be called externally to stop this car
    public void StopAfterCollision()
    {
        stoppedByCollision = true;
        carSpeed = 0f;
    }
} 