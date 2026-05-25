using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float CurrentHealth { get; private set; }

    private bool isDead = false;

    void Start()
    {
        CurrentHealth = maxHealth;
        isDead = false;
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        CurrentHealth -= damage;

        GameManager gm = FindObjectOfType<GameManager>();
        if (gm != null)
        {
            gm.TriggerDamageFlash();
        }

        if (CurrentHealth <= 0f)
        {
            CurrentHealth = 0f;
            Die();
        }
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;
        Debug.Log("Player died");

        GameManager gm = FindObjectOfType<GameManager>();
        if (gm != null)
        {
            gm.ShowLose();
        }
    }
}