using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 40f;

    public Color hitColor = Color.red;
    public float flashDuration = 0.15f;

    public float knockbackDistance = 1.5f;
    public float knockbackDuration = 0.15f;

    private float currentHealth;

    private Renderer enemyRenderer;
    private Color originalColor;

    private CharacterController controller;

    void Start()
    {
        currentHealth = maxHealth;

        controller = GetComponent<CharacterController>();

        enemyRenderer = GetComponent<Renderer>();

        if (enemyRenderer == null)
        {
            enemyRenderer = GetComponentInChildren<Renderer>();
        }

        if (enemyRenderer != null)
        {
            originalColor = enemyRenderer.material.color;
        }
    }

    public void TakeDamage(float damage, Vector3 knockbackDirection)
    {
        currentHealth -= damage;

        Debug.Log(gameObject.name + " took damage. Health: " + currentHealth);

        if (enemyRenderer != null)
        {
            StopCoroutine(nameof(FlashHit));
            StartCoroutine(FlashHit());
        }

        if (controller != null)
        {
            StartCoroutine(Knockback(knockbackDirection));
        }

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    IEnumerator FlashHit()
    {
        enemyRenderer.material.color = hitColor;

        yield return new WaitForSeconds(flashDuration);

        enemyRenderer.material.color = originalColor;
    }

    IEnumerator Knockback(Vector3 direction)
    {
        direction.y = 0f;
        direction.Normalize();

        float elapsed = 0f;
        float speed = knockbackDistance / knockbackDuration;

        while (elapsed < knockbackDuration)
        {
            controller.Move(direction * speed * Time.deltaTime);

            elapsed += Time.deltaTime;

            yield return null;
        }
    }

    void Die()
    {
        GameManager gm = FindObjectOfType<GameManager>();

        if (gm != null)
        {
            gm.EnemyDefeated();
        }

        Destroy(gameObject);
    }
}