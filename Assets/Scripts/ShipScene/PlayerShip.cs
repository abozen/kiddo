using UnityEngine;
using UnityEngine.Events;

public class PlayerShip : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;
    [SerializeField] private float invincibilityDuration = 1f;
    [SerializeField] private GameObject damageEffect;
    [SerializeField] private GameObject deathEffect;

    [Header("Events")]
    public UnityEvent onDamage;
    public UnityEvent onDeath;
    public UnityEvent<float> onHealthChanged; // Passes current health percentage

    private bool isInvincible = false;
    private float invincibilityTimer = 0f;

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    private void Update()
    {
        if (isInvincible)
        {
            invincibilityTimer -= Time.deltaTime;
            if (invincibilityTimer <= 0f)
            {
                isInvincible = false;
            }
        }
    }

    public void TakeDamage(float damage = 20f)
    {
        if (isInvincible) return;

        currentHealth -= damage;
        UpdateHealthUI();

        // Trigger damage effect
        if (damageEffect != null)
        {
            Instantiate(damageEffect, transform.position, Quaternion.identity);
        }

        // Trigger damage event
        onDamage?.Invoke();

        // Check for death
        if (currentHealth <= 0f)
        {
            Die();
        }
        else
        {
            // Start invincibility period
            isInvincible = true;
            invincibilityTimer = invincibilityDuration;
        }
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        UpdateHealthUI();
    }

    private void Die()
    {
        // Trigger death effect
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        // Trigger death event
        onDeath?.Invoke();

        // Disable ship controls
        GetComponent<ShipController>().enabled = false;
        
        // You can either destroy the ship or handle game over state
        // Destroy(gameObject);
        
        // Or handle game over state
        //GameManager.Instance?.GameOver();
    }

    private void UpdateHealthUI()
    {
        float healthPercentage = currentHealth / maxHealth;
        onHealthChanged?.Invoke(healthPercentage);
    }

    public float GetHealthPercentage()
    {
        return currentHealth / maxHealth;
    }

    public bool IsDead()
    {
        return currentHealth <= 0f;
    }
} 