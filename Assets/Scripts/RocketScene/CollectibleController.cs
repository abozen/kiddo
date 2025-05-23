using UnityEngine;

public class CollectibleController : MonoBehaviour
{
    [SerializeField] private GameObject collectEffectPrefab;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Add gem
            GameManager gameManager = FindObjectOfType<GameManager>();
            if (gameManager != null)
            {
                gameManager.AddGem();
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