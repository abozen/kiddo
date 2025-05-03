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
        if (playerTransform.position.z + roadTileLength * 2 > spawnZ - (activeRoadCount - initialRoadCount) * roadTileLength)
        {
            SpawnRoadTile();
        }
        
        // Check if we need to recycle road tiles
        while (roadTiles.Count > 0)
        {
            GameObject oldestRoad = roadTiles.Peek();
            if (oldestRoad.transform.position.z < playerTransform.position.z - recycleDistance)
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
        }
        else
        {
            // Create a new road tile
            roadTile = Instantiate(roadTilePrefab, transform);
        }
        
        // Position the road tile
        roadTile.transform.position = new Vector3(0f, 0f, spawnZ);
        
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