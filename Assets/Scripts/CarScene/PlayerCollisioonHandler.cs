using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisioonHandler : MonoBehaviour
{
    [Header("Collision Effects")]
    [SerializeField] private GameObject collisionEffectPrefab;
    [SerializeField] private GameObject collisionEffectPrefab2;

    [SerializeField] private AudioClip collisionSound;
    [SerializeField] private AudioClip collectableSound;

    [Header("References")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private PlayerCarController playerCarController;

    private int gemsCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
        }

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
        
        if (playerCarController == null)
        {
            playerCarController = GetComponent<PlayerCarController>();
            if (playerCarController == null)
            {
                playerCarController = GetComponentInParent<PlayerCarController>();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Get the contact point from collision
        ContactPoint contact = collision.contacts[0];
        Vector3 collisionPoint = contact.point;
        
        HandleCollision(collision.gameObject, collisionPoint);
    }

    private void OnTriggerEnter(Collider other)
    {
        // For triggers, use the closest point on the collider
        Vector3 collisionPoint = other.ClosestPoint(transform.position);
        
        HandleCollision(other.gameObject, collisionPoint);
    }

    private void HandleCollision(GameObject collisionObject, Vector3 collisionPoint)
    {
        if (collisionObject.CompareTag("TrafficCar"))
        {
            // Stop both cars
            if (playerCarController != null)
            {
                playerCarController.StopAfterCollision();
            }
            
            // Get the AI car component and stop it
            AITrafficCar trafficCar = collisionObject.GetComponent<AITrafficCar>();
            if (trafficCar != null)
            {
                trafficCar.StopAfterCollision();
            }
            
            // Play collision effect at the collision point
            if (collisionEffectPrefab != null)
            {
                GameObject collisionEffect = Instantiate(collisionEffectPrefab, collisionPoint, Quaternion.identity);
                
                // Play collision sound if available
                if (collisionSound != null && audioSource != null)
                {
                    audioSource.PlayOneShot(collisionSound);
                }
            }
            // Play collision effect at the collision point
            if (collisionEffectPrefab2 != null)
            {
                GameObject collisionEffect2 = Instantiate(collisionEffectPrefab2, collisionPoint, Quaternion.identity);
                
            }
            
            // TODO: Game over functionality will be implemented here
            Debug.Log("Game Over - Collided with Traffic Car");
        }
        else if (collisionObject.CompareTag("Collectable"))
        {
            // Get the value from the collectable object
            Collectable collectable = collisionObject.GetComponent<Collectable>();
            if (collectable != null)
            {
                // Add the collectable value to the gems count
                gemsCount += collectable.GetValue();
                Debug.Log("Collected gem. Total: " + gemsCount);
                
                // Play collectable sound if available
                if (collectableSound != null && audioSource != null)
                {
                    audioSource.PlayOneShot(collectableSound);
                }
                
                // Destroy the collected object
                Destroy(collisionObject);
            }
        }
    }

    // Add this method to get the gems count
    public int GetGemsCount()
    {
        return gemsCount;
    }
}
