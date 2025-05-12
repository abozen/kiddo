using System.Collections.Generic;
using UnityEngine;

public class RoadTrafficController : MonoBehaviour
{
    [Header("Spawning Settings")]
    [SerializeField] private float spawnProbability = 0.3f; // Probability to spawn a car at each spawn point
    [SerializeField] private float minTrafficSpeed = 5f;
    [SerializeField] private float maxTrafficSpeed = 15f;
    
    [Header("References")]
    [SerializeField] public GameObject[] trafficCarPrefabs; // Array of traffic car prefabs
    [SerializeField] public Transform carAreasParent; // The parent object containing all CarAreas

    private CarGameManager gameManager;
    private Transform playerTransform;
    private List<CarSpawnArea> spawnAreas = new List<CarSpawnArea>();
    private bool hasSpawned = false;
    
    private void Start()
    {
        // Find references if not assigned
        if (trafficCarPrefabs == null || trafficCarPrefabs.Length == 0)
        {
            Debug.LogError("No traffic car prefabs assigned to RoadTrafficController");
            return;
        }
        
        // Find CarAreas parent if not assigned
        if (carAreasParent == null)
        {
            carAreasParent = transform.Find("CarAreas");
            if (carAreasParent == null)
            {
                Debug.LogError("CarAreas parent not found in road prefab");
                return;
            }
        }
        
        // Find player and game manager
        gameManager = FindObjectOfType<CarGameManager>();
        playerTransform = FindObjectOfType<PlayerCarController>()?.transform;
        
        // Get all spawn areas
        foreach (Transform child in carAreasParent)
        {
            CarSpawnArea spawnArea = child.GetComponent<CarSpawnArea>();
            if (spawnArea == null)
            {
                // Add the component if it doesn't exist
                spawnArea = child.gameObject.AddComponent<CarSpawnArea>();
            }
            spawnAreas.Add(spawnArea);
        }
        
        // Spawn cars when the road is created
        SpawnTrafficCars();
    }
    
    public void SpawnTrafficCars()
    {
        //if (hasSpawned || gameManager == null || !gameManager.IsGameActive() || playerTransform == null)
            //return;

        foreach (CarSpawnArea spawnArea in spawnAreas)
        {
            if(gameManager != null)
            {
                if(gameManager.GetCarCount() > gameManager.maxCarCount)
                {
                    return;
                }
            }
            // Check if we should spawn a car based on probability
            if (Random.value < spawnProbability)
            {
                // Check if the spawn area is already occupied
                if (!spawnArea.IsOccupied())
                {
                    // Randomly select a prefab from the array
                    GameObject selectedPrefab = trafficCarPrefabs[Random.Range(0, trafficCarPrefabs.Length)];
                    
                    // Spawn a car at this position
                    GameObject newCar = Instantiate(selectedPrefab, spawnArea.transform.position, Quaternion.identity);
                    if(gameManager != null)
                    {
                        gameManager.IncreaseCarCount();
                    }
                    
                    // Set the AI car's speed
                    AITrafficCar aiTrafficCar = newCar.GetComponent<AITrafficCar>();
                    if (aiTrafficCar != null)
                    {
                        aiTrafficCar.SetSpeed(Random.Range(minTrafficSpeed, maxTrafficSpeed));
                        // The lane is determined by the spawn position, so we no longer need to set it manually
                    }
                }
            }
        }
        
        hasSpawned = true;
    }
} 