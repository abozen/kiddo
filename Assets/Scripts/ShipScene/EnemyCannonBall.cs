using UnityEngine;

public class EnemyCannonBall : MonoBehaviour
{
    [SerializeField] private float lifetime = 10f;
    [SerializeField] private float airResistance = 0.1f;
    [SerializeField] private GameObject hitEffect;
    [SerializeField] private GameObject waterSplashEffect;

    private Rigidbody rb;
    private bool hasHitWater = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        // Setup physics
        rb.useGravity = true;
        rb.drag = airResistance;
        rb.mass = 10f;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        Destroy(gameObject, lifetime);
    }

    private void FixedUpdate()
    {
        if (rb != null && !hasHitWater)
        {
            // Add slight downward force to simulate air resistance
            rb.AddForce(Vector3.down * 0.5f, ForceMode.Acceleration);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Handle player hit
            PlayerShip playerShip = collision.gameObject.GetComponent<PlayerShip>();
            if (playerShip != null)
            {
                playerShip.TakeDamage();
            }
            SpawnHitEffect();
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Water"))
        {
            // Handle water hit
            hasHitWater = true;
            if (waterSplashEffect != null)
            {
                Instantiate(waterSplashEffect, transform.position, Quaternion.identity);
            }
            Destroy(gameObject, 1f); // Destroy after splash effect
        }
        else
        {
            // Handle other collisions
            SpawnHitEffect();
            Destroy(gameObject);
        }
    }

    private void SpawnHitEffect()
    {
        if (hitEffect != null)
        {
            Instantiate(hitEffect, transform.position, Quaternion.identity);
        }
    }
} 