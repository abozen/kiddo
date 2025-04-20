using UnityEngine;

public class CollectibleController : MonoBehaviour
{
    [SerializeField] private int scoreValue = 1;
    [SerializeField] private GameObject collectEffectPrefab;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Add score
            GameManager gameManager = FindObjectOfType<GameManager>();
            if (gameManager != null)
            {
                gameManager.AddScore(scoreValue);
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