using UnityEngine;
using System.Collections;

public class EnemyShip : MonoBehaviour
{
    [Header("Cannon Settings")]
    [SerializeField] private Transform[] cannons;
    [SerializeField] private GameObject cannonBallPrefab;
    [SerializeField] private float minFireRate = 3f;
    [SerializeField] private float maxFireRate = 7f;
    [SerializeField] private float initialVelocity = 30f;
    [SerializeField] private float maxAngle = 45f;
    [SerializeField] private float accuracyRadius = 5f; // Radius around player where cannon balls can land
    [SerializeField] private float cannonAngleEffector = 0.1f;

    [Header("References")]
    [SerializeField] private float maxDistanceToPlayer;
    private Transform playerShip;
    private bool canFire = true;

    private void Start()
    {
        playerShip = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (playerShip == null)
        {
            Debug.LogError("Player ship not found!");
            return;
        }

        StartCoroutine(FireRoutine());
    }

    private IEnumerator FireRoutine()
    {
        while (true)
        {
            if (canFire && playerShip != null)
            {
                FireCannon();
                yield return new WaitForSeconds(Random.Range(minFireRate, maxFireRate));
            }
            else
            {
                yield return new WaitForSeconds(1f);
            }
        }
    }

    private void FireCannon()
    {
        if (cannons == null || cannons.Length == 0) return;

        // Select random cannon
        Transform selectedCannon = cannons[Random.Range(0, cannons.Length)];

        // Calculate target position with some randomness
        Vector3 targetPosition = playerShip.position;
        targetPosition += Random.insideUnitSphere * accuracyRadius;
        targetPosition.y = 0; // Keep target at water level

        // Calculate direction to target
        Vector3 directionToTarget = (targetPosition - selectedCannon.position).normalized;
        float distanceToTarget = Vector3.Distance(targetPosition, transform.position);

        HandleDistanceDestroy(distanceToTarget);    //if distance is too much destroy ship

        // Calculate launch angle (random between 30 and maxAngle degrees)
        float launchAngle = 35f;// Random.Range(30f, maxAngle);
        Vector3 launchDirection = Quaternion.Euler(-launchAngle, 0, 0) * directionToTarget;

        // Spawn cannon ball
        GameObject cannonBall = Instantiate(cannonBallPrefab, selectedCannon.position, Quaternion.LookRotation(launchDirection));
        Rigidbody rb = cannonBall.GetComponent<Rigidbody>();

        if (rb != null)
        {
            // Apply initial velocity
            rb.velocity = launchDirection * initialVelocity * CalculateInitialVelocity(distanceToTarget); //Mathf.Sqrt(distanceToTarget) ;

            // Add some random rotation to the cannon ball
            rb.angularVelocity = Random.insideUnitSphere * 2f;
        }
    }

    void HandleDistanceDestroy(float distance)
    {
        if(distance > maxDistanceToPlayer)
        {
            Destroy(gameObject);
        }
    }
    float CalculateInitialVelocity(float distance)
    {
        float angleInDegrees = 35f;
        float gravity = 9.81f;
        float angleInRadians = angleInDegrees * Mathf.Deg2Rad;

        float velocity = Mathf.Sqrt((distance * gravity) / Mathf.Sin(2 * angleInRadians));

        return velocity;
    }


    public void TakeDamage()
    {
        // Handle damage
        Destroy(gameObject);
    }
}

