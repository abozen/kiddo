using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private GameObject deathEffect;
    [SerializeField] private float health = 1f;

    public void TakeDamage()
    {
        health--;
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
} 