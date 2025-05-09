using System.Collections.Generic;
using UnityEngine;

public class RoadManager : MonoBehaviour
{
    [Header("Road Settings")]
    [SerializeField] private GameObject roadTilePrefab;
    [SerializeField] private int initialRoadCount = 5;
    [SerializeField] private int activeRoadCount = 7; // How many active road pieces to maintain
    [SerializeField] private float roadTileLength = 50f; // Length of each road tile
    [SerializeField] private float recycleDistance = 40f; // Distance behind player to recycle roads
    [SerializeField] private float roadSpawnOffset = 20f; // Spawn road tiles ahead of player to prevent visual pop-in
    [SerializeField] private float roadRecycleOffset = 10f; // Distance behind player to start recycling
    
    [Header("Traffic Settings")]
    [SerializeField] private GameObject trafficCarPrefab; // Reference to the traffic car prefab
    
    [Header("Collectable Settings")]
    [SerializeField] private GameObject collectablePrefab; // Reference to the collectable prefab
    
    [Header("References")]
    [SerializeField] private Transform playerTransform;
    
    private Queue<GameObject> roadTiles = new Queue<GameObject>();
    private float spawnZ = 0f;
    
    private void Start()
    {
        // Find the player if not assigned
        if (playerTransform == null)
        {
            PlayerCarController player = FindObjectOfType<PlayerCarController>();
            if (player != null)
            {
                playerTransform = player.transform;
            }
        }
        
        // Set initial spawn position based on player position plus offset
        if (playerTransform != null)
        {
            spawnZ = playerTransform.position.z - roadTileLength; // Start one tile behind player
        }
        
        // Create initial road tiles
        for (int i = 0; i < initialRoadCount; i++)
        {
            SpawnRoadTile();
        }
    }
    
    private void Update()
    {
        if (playerTransform == null)
            return;
            
        ManageRoadTiles();
    }
    
    private void ManageRoadTiles()
    {
        // Check if we need to spawn more road tiles
        if (playerTransform.position.z + roadSpawnOffset > spawnZ - (activeRoadCount - initialRoadCount) * roadTileLength)
        {
            SpawnRoadTile();
        }
        
        // Check if we need to recycle road tiles
        while (roadTiles.Count > 0)
        {
            GameObject oldestRoad = roadTiles.Peek();
            if (oldestRoad.transform.position.z < playerTransform.position.z - recycleDistance - roadRecycleOffset)
            {
                RecycleRoadTile();
            }
            else
            {
                break;
            }
        }
    }
    
    private void SpawnRoadTile(bool isRecycled = false)
    {
        GameObject roadTile;
        
        if (isRecycled)
        {
            // Reuse the oldest road tile
            roadTile = roadTiles.Dequeue();
            
            // Reset hasSpawned flag on the traffic controller when recycling
            // RoadTrafficController trafficController2 = roadTile.GetComponent<RoadTrafficController>();
            // if (trafficController2 != null)
            // {
            //     Destroy(trafficController2);
            // }
        }
        else
        {
            // Create a new road tile
            roadTile = Instantiate(roadTilePrefab, transform);
        }
        
        // Position the road tile
        roadTile.transform.position = new Vector3(0f, 0f, spawnZ);
        
        // Add and setup RoadTrafficController
        RoadTrafficController trafficController = roadTile.GetComponent<RoadTrafficController>();
        if (trafficController == null)
        {
            trafficController = roadTile.AddComponent<RoadTrafficController>();
            trafficController.trafficCarPrefab = this.trafficCarPrefab;
            trafficController.carAreasParent = roadTile.transform.Find("CarAreas");
        }
        
        // Add and setup CollectableSpawner
        CollectableSpawner collectableSpawner = roadTile.GetComponent<CollectableSpawner>();
        if (collectableSpawner == null)
        {
            collectableSpawner = roadTile.AddComponent<CollectableSpawner>();
        }
        
        // If we have a collectable prefab reference, set it on the spawner
        if (collectablePrefab != null)
        {
            System.Reflection.FieldInfo collectableField = typeof(CollectableSpawner).GetField("collectablePrefab", 
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (collectableField != null)
            {
                collectableField.SetValue(collectableSpawner, collectablePrefab);
            }
        }
        
        if(isRecycled)
        {
            trafficController.SpawnTrafficCars();
            
            // Reset and spawn new collectables when recycling
            collectableSpawner.ResetSpawner();
            collectableSpawner.SpawnCollectables();
        }
        else
        {
            // For new tiles, just spawn the collectables
            collectableSpawner.SpawnCollectables();
        }
        
        // Set the traffic car prefab reference (using SerializedField)
        if (trafficCarPrefab != null)
        {
            // We need to use reflection or a public property to set this
            System.Reflection.FieldInfo field = typeof(RoadTrafficController).GetField("trafficCarPrefab", 
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (field != null)
            {
                field.SetValue(trafficController, trafficCarPrefab);
            }
        }
        
        // Add to the queue
        roadTiles.Enqueue(roadTile);
        
        // Update the next spawn position
        spawnZ += roadTileLength;
    }
    
    private void RecycleRoadTile()
    {
        if (roadTiles.Count <= 0)
            return;
            
        SpawnRoadTile(true);
    }
} 