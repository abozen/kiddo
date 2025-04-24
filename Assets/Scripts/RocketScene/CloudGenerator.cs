using System.Collections.Generic;
using UnityEngine;

public class CloudGenerator : MonoBehaviour
{
    [Header("Cloud Settings")]
    [SerializeField] private GameObject[] cloudPrefabs;
    [SerializeField] private int maxCloudCount = 20;
    [SerializeField] private float minCloudScale = 0.5f;
    [SerializeField] private float maxCloudScale = 2f;
    
    [Header("Spawn Settings")]
    [SerializeField] private float spawnDistance = 100f;
    [SerializeField] private float destroyDistance = 20f;
    [SerializeField] private float minSpawnInterval = 0.5f;
    [SerializeField] private float maxSpawnInterval = 2f;
    [SerializeField] private float horizSpawnRange = 15f;
    [SerializeField] private float vertSpawnRange = 10f;
    [SerializeField] private Transform player;
    
    private List<GameObject> cloudPool;
    private float nextSpawnTime;
    private int currentCloudIndex = 0;
    
    private void Start()
    {
        InitializeCloudPool();
    }
    
    private void InitializeCloudPool()
    {
        cloudPool = new List<GameObject>();
        
        // Create initial cloud pool
        for (int i = 0; i < maxCloudCount; i++)
        {
            CreateNewCloud();
        }
    }
    
    private void CreateNewCloud()
    {
        if (cloudPrefabs.Length == 0) return;
        
        // Randomly select cloud prefab
        GameObject cloudPrefab = cloudPrefabs[Random.Range(0, cloudPrefabs.Length)];
        
        // Instantiate cloud
        GameObject cloud = Instantiate(cloudPrefab, Vector3.zero, Quaternion.identity);
        cloud.SetActive(false);
        cloudPool.Add(cloud);
    }
    
    private void Update()
    {
        if (player == null) return;
        
        // Spawn new clouds
        if (Time.time >= nextSpawnTime)
        {
            SpawnCloud();
            nextSpawnTime = Time.time + Random.Range(minSpawnInterval, maxSpawnInterval);
        }
        
        // Clean up clouds that are behind the player
        CleanupClouds();
    }
    
    private void SpawnCloud()
    {
        if (cloudPool.Count == 0) return;
        
        // Get next cloud from pool
        GameObject cloud = cloudPool[currentCloudIndex];
        currentCloudIndex = (currentCloudIndex + 1) % cloudPool.Count;
        
        // Determine spawn position
        Vector3 spawnPos = player.position + player.forward * spawnDistance;
        
        // Randomize position
        spawnPos.x = Random.Range(-horizSpawnRange, horizSpawnRange);
        spawnPos.y = Random.Range(-vertSpawnRange, vertSpawnRange);
        
        // Randomize scale
        float scale = Random.Range(minCloudScale, maxCloudScale);
        cloud.transform.localScale = new Vector3(scale, scale, scale);
        
        // Randomize rotation
        cloud.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
        
        // Set position and activate
        cloud.transform.position = spawnPos;
        cloud.SetActive(true);
    }
    
    private void CleanupClouds()
    {
        foreach (GameObject cloud in cloudPool)
        {
            if (cloud.activeSelf && player.position.z - cloud.transform.position.z > destroyDistance)
            {
                cloud.SetActive(false);
            }
        }
    }
} 