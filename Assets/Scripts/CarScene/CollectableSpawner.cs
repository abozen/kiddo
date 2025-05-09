using System.Collections.Generic;
using UnityEngine;

public class CollectableSpawner : MonoBehaviour
{
    [Header("Collectable Prefabs")]
    [SerializeField] private GameObject collectablePrefab;

    [Header("Spawn Settings")]
    [SerializeField] private float bronzeSpawnProbability = 0.2f;
    [SerializeField] private float silverSpawnProbability = 0.1f;
    [SerializeField] private float goldSpawnProbability = 0.05f;

    [SerializeField] private float collectableHeight = 0f;
    [SerializeField] private float laneWidth = 4.0f;
    [SerializeField] private int laneCount = 4;
    [SerializeField] private float startXPosition = -6.0f; // Leftmost lane position

    private bool hasSpawned = false;
    private Transform roadTransform;

    private void Start()
    {
        roadTransform = transform;
    }

    public void SpawnCollectables()
    {
        if (hasSpawned || collectablePrefab == null)
            return;

        hasSpawned = true;

        for (int lane = 0; lane < laneCount; lane++)
        {
            // Calculate lane position
            float xPosition = startXPosition + (lane * laneWidth);
            
            // Randomly determine if we spawn a collectable in this lane
            float randomValue = Random.value;
            
            if (randomValue < goldSpawnProbability)
            {
                SpawnCollectable(xPosition, Collectable.CollectableType.Gold);
            }
            else if (randomValue < goldSpawnProbability + silverSpawnProbability)
            {
                SpawnCollectable(xPosition, Collectable.CollectableType.Silver);
            }
            else if (randomValue < goldSpawnProbability + silverSpawnProbability + bronzeSpawnProbability)
            {
                SpawnCollectable(xPosition, Collectable.CollectableType.Bronze);
            }
        }
    }

    private void SpawnCollectable(float xPosition, Collectable.CollectableType type)
    {
        // Add slight randomization to z position to avoid collectables lined up perfectly
        float zOffset = Random.Range(-5f, 5f);
        
        Vector3 spawnPosition = new Vector3(
            xPosition, 
            collectableHeight, 
            transform.position.z + zOffset
        );

        GameObject collectableObject = Instantiate(collectablePrefab, spawnPosition, Quaternion.identity, transform);
        Collectable collectable = collectableObject.GetComponent<Collectable>();
        
        if (collectable != null)
        {
            collectable.type = type;
        }
    }

    public void ResetSpawner()
    {
        hasSpawned = false;
    }
} 