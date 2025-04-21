using System.Collections.Generic;
using UnityEngine;

public class EnvironmentGenerator : MonoBehaviour
{
    [Header("Environment Settings")]
    [SerializeField] private GameObject[] environmentPrefabs;
    [SerializeField] private float environmentLength = 100f;
    [SerializeField] private Transform player;
    
    private List<GameObject> activeEnvironments = new List<GameObject>();
    private float lastSpawnZ = 0f;
    
    private void Start()
    {
        // Spawn initial environment
        SpawnEnvironment(0f);
    }
    
    private void Update()
    {
        if (player == null) return;
        
        // Check if we need to spawn new environment
        if (player.position.z > lastSpawnZ - environmentLength)
        {
            SpawnEnvironment(lastSpawnZ + environmentLength);
        }
        
        // Clean up old environments
        CleanupEnvironments();
    }
    
    private void SpawnEnvironment(float zPosition)
    {
        // Randomly select environment prefab
        GameObject envPrefab = environmentPrefabs[Random.Range(0, environmentPrefabs.Length)];
        
        // Create new environment
        Vector3 spawnPos = new Vector3(-200f, 400f, zPosition);
        GameObject newEnv = Instantiate(envPrefab, spawnPos, Quaternion.identity);
        activeEnvironments.Add(newEnv);
        
        lastSpawnZ = zPosition;
    }
    
    private void CleanupEnvironments()
    {
        for (int i = activeEnvironments.Count - 1; i >= 0; i--)
        {
            GameObject env = activeEnvironments[i];
            
            if (env == null)
            {
                activeEnvironments.RemoveAt(i);
                continue;
            }
            
            // Remove environments that are too far behind
            if (player.position.z - env.transform.position.z > environmentLength * 2)
            {
                Destroy(env);
                activeEnvironments.RemoveAt(i);
            }
        }
    }
} 