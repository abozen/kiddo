using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject[] enemyShipPrefabs; // Array of different enemy ship prefabs
    [SerializeField] private float spawnRadius = 50f;
    [SerializeField] private float minSpawnDistance = 30f;
    [SerializeField] private int maxEnemies = 5;
    [SerializeField] private float spawnInterval = 10f;
    [SerializeField] private float spawnAngleOffset = 45f; // Angle offset from camera's view direction

    private Transform playerShip;
    private Camera mainCamera;
    private int currentEnemyCount;
    private const int MAX_SPAWN_ATTEMPTS = 10;

    private void Start()
    {
        // Find player ship
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerShip = player.transform;
            mainCamera = Camera.main;
            StartCoroutine(SpawnEnemyRoutine());
        }
        else
        {
            Debug.LogError("Player ship not found! Make sure it has the 'Player' tag.");
        }

        // Validate enemy prefabs
        if (enemyShipPrefabs == null || enemyShipPrefabs.Length == 0)
        {
            Debug.LogError("No enemy ship prefabs assigned to the spawner!");
        }
    }

    private IEnumerator SpawnEnemyRoutine()
    {
        while (true)
        {
            if (currentEnemyCount < maxEnemies && playerShip != null)
            {
                SpawnEnemy();
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private Vector3 GetSpawnPositionOutsideCameraView()
    {
        for (int attempt = 0; attempt < MAX_SPAWN_ATTEMPTS; attempt++)
        {
            // Get camera's forward direction
            Vector3 cameraForward = mainCamera.transform.forward;
            cameraForward.y = 0;
            cameraForward.Normalize();

            // Calculate spawn angle relative to camera's view direction
            float randomAngle = Random.Range(180f - spawnAngleOffset, 180f + spawnAngleOffset);
            Vector3 spawnDirection = Quaternion.Euler(0, randomAngle, 0) * cameraForward;

            // Calculate spawn position
            float randomDistance = Random.Range(minSpawnDistance, spawnRadius);
            Vector3 spawnPosition = playerShip.position + spawnDirection * randomDistance;
            spawnPosition.y = 0; // Keep at water level

            // Check if the position is outside camera view
            Vector3 viewportPoint = mainCamera.WorldToViewportPoint(spawnPosition);
            if (viewportPoint.x < 0 || viewportPoint.x > 1 || viewportPoint.y < 0 || viewportPoint.y > 1 || viewportPoint.z < 0)
            {
                return spawnPosition;
            }
        }

        // If all attempts fail, spawn at a default position behind the camera
        Vector3 defaultDirection = -mainCamera.transform.forward;
        defaultDirection.y = 0;
        defaultDirection.Normalize();
        return playerShip.position + defaultDirection * minSpawnDistance;
    }

    private void SpawnEnemy()
    {
        // Get spawn position outside camera view
        Vector3 spawnPosition = GetSpawnPositionOutsideCameraView();

        // Calculate direction from enemy to player
        Vector3 directionToPlayer = playerShip.position - spawnPosition;
        directionToPlayer.y = 0; // Keep rotation on horizontal plane
        directionToPlayer.Normalize();

        // Calculate perpendicular rotation (90 degrees to the right)
        Quaternion perpendicularRotation = Quaternion.LookRotation(directionToPlayer) * Quaternion.Euler(0, 90, 0);

        // Randomly select an enemy prefab
        GameObject selectedPrefab = enemyShipPrefabs[Random.Range(0, enemyShipPrefabs.Length)];
        GameObject enemy = Instantiate(selectedPrefab, spawnPosition, perpendicularRotation);
        currentEnemyCount++;

        // Subscribe to enemy destruction
        EnemyShip enemyShip = enemy.GetComponent<EnemyShip>();
        if (enemyShip != null)
        {
            // You can add an event in EnemyShip later to handle this
            // enemyShip.OnDestroyed += HandleEnemyDestroyed;
        }
    }

    private void HandleEnemyDestroyed()
    {
        currentEnemyCount--;
    }
} 