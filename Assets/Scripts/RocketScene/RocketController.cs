using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float baseForwardSpeed = 15f;
    [SerializeField] private float baseHorizontalSpeed = 10f;
    [SerializeField] private float horizontalLimit = 5f;
    [SerializeField] private float smoothFactorHorizantal;
    
    [Header("Speed Increase Settings")]
    [SerializeField] private float speedIncreaseRate = 0.1f;
    [SerializeField] private float maxSpeedMultiplier = 2.5f;
    [SerializeField] private float speedIncreaseInterval = 10f;
    
    [Header("Shooting Settings")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletSpawnPoint;
    [SerializeField] private float shootCooldown = 0.5f;
    
    [Header("Game Control")]
    [SerializeField] private GameManager gameManager;
    
    private float shootTimer;
    private Rigidbody rb;
    private float currentXVelocity;
    private float currentSpeedMultiplier = 1f;
    private float nextSpeedIncreaseTime;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
        }
        nextSpeedIncreaseTime = Time.time + speedIncreaseInterval;
    }
    
    private void Update()
    {
        if (gameManager.IsGameOver())
            return;
            
        // Handle keyboard input (for PC/Editor)
        float horizontalInput = Input.GetAxis("Horizontal");
        if (horizontalInput != 0)
        {
            MoveHorizontal(horizontalInput);
        }
        
        // Update shoot cooldown timer
        if (shootTimer > 0)
        {
            shootTimer -= Time.deltaTime;
        }
        
        // Increase speed over time
        if (Time.time >= nextSpeedIncreaseTime && currentSpeedMultiplier < maxSpeedMultiplier)
        {
            currentSpeedMultiplier += speedIncreaseRate;
            nextSpeedIncreaseTime = Time.time + speedIncreaseInterval;
        }
    }
    
    private void FixedUpdate()
    {
        if (gameManager.IsGameOver())
            return;
            
        // Constant forward movement with speed multiplier
        float currentForwardSpeed = baseForwardSpeed * currentSpeedMultiplier;
        rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, currentForwardSpeed);
        
        // Limit horizontal position
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, -horizontalLimit, horizontalLimit);
        transform.position = pos;
    }
    
    public void MoveHorizontal(float direction)
    {
        if (gameManager.IsGameOver())
            return;

        // Calculate horizontal speed with multiplier
        float currentHorizontalSpeed = baseHorizontalSpeed * currentSpeedMultiplier;
        float targetXVelocity = direction * currentHorizontalSpeed;

        // Lerp ile yumuşak geçiş
        currentXVelocity = Mathf.Lerp(currentXVelocity, targetXVelocity, Time.deltaTime * smoothFactorHorizantal);

        // Yeni velocity ile uygulama
        rb.velocity = new Vector3(currentXVelocity, rb.velocity.y, rb.velocity.z);
    }
    
    public void Shoot()
    {
        if (gameManager.IsGameOver() || shootTimer > 0)
            return;
            
        // Create bullet
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        
        // Reset cooldown
        shootTimer = shootCooldown;
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            // Game over when colliding with obstacles
            gameManager.GameOver();
            
            // Add explosion effect here if desired
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}
