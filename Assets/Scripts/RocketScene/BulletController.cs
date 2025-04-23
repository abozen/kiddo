using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField] private float speed = 30f;
    [SerializeField] private float lifetime = 3f;
    [SerializeField] private GameObject hitEffectPrefab;
    [SerializeField] private GameObject obstaclePopPrefab;

    
    private Rigidbody rb;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        Destroy(gameObject, lifetime); // Self-destruct after lifetime
    }
    
    private void Start()
    {
        // Move forward
        rb.velocity = transform.forward * speed;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            // Destroy obstacle
            Destroy(other.gameObject);
            
            // Add score if GameManager exists
            // GameManager gameManager = FindObjectOfType<GameManager>();
            // if (gameManager != null)
            // {
            //     gameManager.AddScore(10);
            // }
            
            // Create hit effect if available
            if (hitEffectPrefab != null)
            {
                Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
            }
            if (obstaclePopPrefab != null)
            {
                Instantiate(obstaclePopPrefab, transform.position, Quaternion.identity);
            }
            
            // Destroy self
            Destroy(gameObject);
        }
    }
}