using UnityEngine;

public class BulletCollectibleController : MonoBehaviour
{
    [SerializeField] private GameObject collectEffectPrefab;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Add bullet
            GameManager gameManager = FindObjectOfType<GameManager>();
            if (gameManager != null)
            {
                gameManager.AddBullet();
            }
            
            // Spawn collect effect
            if (collectEffectPrefab != null)
            {
                Instantiate(collectEffectPrefab, transform.position, Quaternion.identity);
            }
            
            // Destroy collectible
            Destroy(gameObject);
        }
    }
} 