using System.Collections.Generic;
using UnityEngine;

public class ObstacleGenerator : MonoBehaviour
{
    [System.Serializable]
    public class ObstaclePattern
    {
        public GameObject[] obstaclePrefabs;
        public Vector3[] positions;
    }
    
    [Header("Obstacle Settings")]
    [SerializeField] private GameObject[] obstaclePrefabs;
    [SerializeField] private ObstaclePattern[] obstaclePatterns;
    
    [Header("Collectible Settings")]
    [SerializeField] private GameObject collectiblePrefab;
    [SerializeField] private float collectibleSpawnChance = 0.3f;
    [SerializeField] private GameObject bulletCollectiblePrefab;
    [SerializeField] private float bulletCollectibleSpawnChance = 0.2f;
    
    [Header("Generation Settings")]
    [SerializeField] private float spawnDistance = 100f;
    [SerializeField] private float destroyDistance = 20f;
    [SerializeField] private float minSpawnInterval = 1f;
    [SerializeField] private float maxSpawnInterval = 3f;
    [SerializeField] private float horizSpawnRange = 5f;
    [SerializeField] private Transform player;
    
    [Header("Difficulty Settings")]
    [SerializeField] private float difficultyIncreaseRate = 0.1f;
    [SerializeField] private float minSpawnIntervalLimit = 0.5f;
    
    private float nextSpawnTime;
    private float difficultyFactor = 1f;
    private List<GameObject> activeObstacles = new List<GameObject>();
    
    private void Update()
    {
        if (player == null)
            return;
            
        // Spawn new obstacles
        if (Time.time >= nextSpawnTime)
        {
            SpawnObstacle();
            
            // Try to spawn collectibles
            if (Random.value < collectibleSpawnChance)
            {
                SpawnCollectible();
            }
            
            if (Random.value < bulletCollectibleSpawnChance)
            {
                SpawnBulletCollectible();
            }
            
            // Calculate next spawn time with difficulty
            float interval = Mathf.Lerp(maxSpawnInterval, minSpawnInterval, difficultyFactor);
            interval = Mathf.Max(interval, minSpawnIntervalLimit);
            nextSpawnTime = Time.time + interval;
            
            // Increase difficulty over time
            difficultyFactor += difficultyIncreaseRate * Time.deltaTime;
            difficultyFactor = Mathf.Min(difficultyFactor, 1f);
        }
        
        // Clean up obstacles that are behind the player
        CleanupObstacles();
    }
    
    private void SpawnObstacle()
    {
        // Determine spawn position ahead of player
        Vector3 spawnPos = player.position + player.forward * spawnDistance;
        
        // Randomly choose between pattern or single obstacle
        if (obstaclePatterns.Length > 0 && Random.value > 0.5f)
        {
            SpawnPattern(spawnPos);
        }
        else
        {
            SpawnSingleObstacle(spawnPos);
        }
    }
    
    private void SpawnSingleObstacle(Vector3 basePosition)
    {
        // Randomly select obstacle prefab
        GameObject obstaclePrefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];
        
        // Randomize X position
        Vector3 position = basePosition;
        position.x = Random.Range(-horizSpawnRange, horizSpawnRange);
        position.y -= 12f;
        
        // Instantiate obstacle
        GameObject obstacle = Instantiate(obstaclePrefab, position, Quaternion.Euler(0, 180, 0));
        activeObstacles.Add(obstacle);
    }
    
    private void SpawnPattern(Vector3 basePosition)
    {
        // Select random pattern
        ObstaclePattern pattern = obstaclePatterns[Random.Range(0, obstaclePatterns.Length)];
        
        // Spawn all objects in pattern
        for (int i = 0; i < pattern.positions.Length; i++)
        {
            int prefabIndex = i < pattern.obstaclePrefabs.Length ? i : Random.Range(0, pattern.obstaclePrefabs.Length);
            GameObject prefab = pattern.obstaclePrefabs[prefabIndex];
            
            Vector3 position = basePosition + pattern.positions[i];
            GameObject obstacle = Instantiate(prefab, position, Quaternion.identity);
            activeObstacles.Add(obstacle);
        }
    }
    
    private void SpawnCollectible()
    {
        if (collectiblePrefab == null) return;
        
        // Determine spawn position ahead of player
        Vector3 spawnPos = player.position + player.forward * spawnDistance;
        
        // Randomize X position
        spawnPos.x = Random.Range(-horizSpawnRange, horizSpawnRange);
        spawnPos.y = .5f; // Adjust this based on your game's needs
        
        // Instantiate collectible
        Instantiate(collectiblePrefab, spawnPos, Quaternion.identity);
    }
    
    private void SpawnBulletCollectible()
    {
        if (bulletCollectiblePrefab == null) return;
        
        // Determine spawn position ahead of player
        Vector3 spawnPos = player.position + player.forward * spawnDistance;
        
        // Randomize X position
        spawnPos.x = Random.Range(-horizSpawnRange, horizSpawnRange);
        spawnPos.y = .5f; // Adjust this based on your game's needs
        
        // Instantiate bullet collectible
        Instantiate(bulletCollectiblePrefab, spawnPos, Quaternion.identity);
    }
    
    private void CleanupObstacles()
    {
        for (int i = activeObstacles.Count - 1; i >= 0; i--)
        {
            GameObject obstacle = activeObstacles[i];
            
            // Skip if obstacle was already destroyed
            if (obstacle == null)
            {
                activeObstacles.RemoveAt(i);
                continue;
            }
            
            // Check if obstacle is behind player by required distance
            if (player.position.z - obstacle.transform.position.z > destroyDistance)
            {
                Destroy(obstacle);
                activeObstacles.RemoveAt(i);
            }
        }
    }
}