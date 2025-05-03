using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficCarSpawner : MonoBehaviour
{
    [Header("Spawning Settings")]
    [SerializeField] private float spawnInterval = 1.5f;
    [SerializeField] private float spawnDistance = 100f;
    [SerializeField] private float minTrafficSpeed = 5f;
    [SerializeField] private float maxTrafficSpeed = 15f;
    [SerializeField] private float minSpawnIntervalBetweenLanes = 2f; // Minimum interval between spawns in the same lane
    [SerializeField] private int laneCount = 4; // Number of lanes to spawn traffic in
    
    [Header("References")]
    [SerializeField] private GameObject trafficCarPrefab;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private CarGameManager gameManager;
    
    private List<float> lastSpawnTimePerLane;
    private float lastSpawnTime;
    
    private void Start()
    {
        // Initialize the list for tracking spawn times per lane
        lastSpawnTimePerLane = new List<float>();
        for (int i = 0; i < laneCount; i++)
        {
            lastSpawnTimePerLane.Add(0f);
        }
        
        // Try to find player and game manager if not assigned
        if (playerTransform == null)
        {
            PlayerCarController player = FindObjectOfType<PlayerCarController>();
            if (player != null)
            {
                playerTransform = player.transform;
            }
        }
        
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<CarGameManager>();
        }
        
        lastSpawnTime = Time.time;
        StartCoroutine(SpawnTrafficCars());
    }
    
    private IEnumerator SpawnTrafficCars()
    {
        while (true)
        {
            if (gameManager != null && gameManager.IsGameActive() && playerTransform != null)
            {
                if (Time.time >= lastSpawnTime + spawnInterval)
                {
                    SpawnTrafficCar();
                    lastSpawnTime = Time.time;
                }
            }
            yield return null;
        }
    }
    
    private void SpawnTrafficCar()
    {
        List<int> availableLanes = GetAvailableLanes();
        
        if (availableLanes.Count > 0)
        {
            // Randomly select a lane from available lanes
            int laneIndex = availableLanes[Random.Range(0, availableLanes.Count)];
            
            // Calculate the spawn position
            Vector3 spawnPosition = new Vector3(
                0f, // X position will be set by the AITrafficCar.SetLane method
                playerTransform.position.y,
                playerTransform.position.z + spawnDistance
            );
            
            // Spawn the traffic car
            GameObject newCar = Instantiate(trafficCarPrefab, spawnPosition, Quaternion.identity);
            
            // Set the AI car's speed and lane
            AITrafficCar aiTrafficCar = newCar.GetComponent<AITrafficCar>();
            if (aiTrafficCar != null)
            {
                aiTrafficCar.SetSpeed(Random.Range(minTrafficSpeed, maxTrafficSpeed));
                aiTrafficCar.SetLane(laneIndex);
            }
            
            // Update the last spawn time for this lane
            lastSpawnTimePerLane[laneIndex] = Time.time;
        }
    }
    
    private List<int> GetAvailableLanes()
    {
        List<int> availableLanes = new List<int>();
        float currentTime = Time.time;
        
        // Check each lane
        for (int i = 0; i < laneCount; i++)
        {
            if (currentTime - lastSpawnTimePerLane[i] >= minSpawnIntervalBetweenLanes)
            {
                availableLanes.Add(i);
            }
        }
        
        return availableLanes;
    }
} 