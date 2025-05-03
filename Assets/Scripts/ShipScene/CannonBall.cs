using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CannonBall : MonoBehaviour
{
    [Header("Visual Effects")]
    [SerializeField] private GameObject impactEffectPrefab;
    [SerializeField] private GameObject trailEffectPrefab;
    [SerializeField] private float minImpactForce = 5f;

    [Header("Physical Properties")]
    [SerializeField] private float mass = 8f;
    [SerializeField] private float drag = 0f;
    [SerializeField] private PhysicMaterial physicsMaterial;
    [SerializeField] private float lifetime = 5f;


    private Rigidbody rb;
    private GameObject trailEffect;

    private void Awake()
    {
        Destroy(gameObject, lifetime);
        rb = GetComponent<Rigidbody>();
        SetupPhysics();
        CreateTrailEffect();
    }

    private void SetupPhysics()
    {
        // Set physical properties for realistic cannonball behavior
        rb.mass = mass;
        rb.drag = drag;
        rb.useGravity = true;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        
        // Add sphere collider if not present
        SphereCollider collider = GetComponent<SphereCollider>();
        if (collider == null)
        {
            collider = gameObject.AddComponent<SphereCollider>();
        }
        
        // Apply physics material if available
        if (physicsMaterial != null)
        {
            collider.material = physicsMaterial;
        }
    }

    private void CreateTrailEffect()
    {
        if (trailEffectPrefab != null)
        {
            trailEffect = Instantiate(trailEffectPrefab, transform.position, Quaternion.identity);
            trailEffect.transform.SetParent(transform);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Create impact effect based on collision force
        if (impactEffectPrefab != null && collision.relativeVelocity.magnitude > minImpactForce)
        {
            Vector3 impactPoint = collision.contacts[0].point;
            Quaternion impactRotation = Quaternion.LookRotation(collision.contacts[0].normal);
            
            GameObject impactEffect = Instantiate(impactEffectPrefab, impactPoint, impactRotation);
            
            // Scale effect based on impact force
            float impactScale = Mathf.Clamp01(collision.relativeVelocity.magnitude / 30f);
            impactEffect.transform.localScale *= (0.7f + impactScale);
            
            // Destroy effect after some time
            Destroy(impactEffect, 2f);
        }
        
        // Apply impulse force to hit objects if they have rigidbody
        Rigidbody hitRb = collision.gameObject.GetComponent<Rigidbody>();
        if (hitRb != null && !hitRb.isKinematic)
        {
            Vector3 impactForce = collision.relativeVelocity.normalized * rb.mass * collision.relativeVelocity.magnitude;
            hitRb.AddForceAtPosition(impactForce * 0.8f, collision.contacts[0].point, ForceMode.Impulse);
        }
    }
}