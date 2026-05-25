using UnityEngine;

public class EnemyChase : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float rotationSpeed = 5f;
    public float stopDistance = 1.0f;
    public float attackRange = 1.5f;

    public float attackDamage = 5f;
    public float attackCooldown = 1.5f;

    public float separationRadius = 2f;
    public float separationStrength = 1.5f;

    private float attackTimer;

    private Transform target;
    private CharacterController controller;
    private PlayerHealth playerHealth;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            target = playerObject.transform;
            playerHealth = playerObject.GetComponent<PlayerHealth>();
        }

        attackTimer = Random.Range(0f, attackCooldown);
    }

    void Update()
    {
        if (target == null || playerHealth == null) return;

        attackTimer -= Time.deltaTime;

        Vector3 direction = target.position - transform.position;
        direction.y = 0f;

        float distance = direction.magnitude;

        // Rotate toward player
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        // Attack
        if (distance <= attackRange && attackTimer <= 0f)
        {
            playerHealth.TakeDamage(attackDamage);
            attackTimer = attackCooldown + Random.Range(0f, 0.5f);
            return;
        }

        // Movement + spacing
        if (distance > stopDistance)
        {
            Vector3 moveDir = direction.normalized;

            Collider[] nearby = Physics.OverlapSphere(transform.position, separationRadius);

            foreach (Collider other in nearby)
            {
                if (other.gameObject != gameObject && other.GetComponent<EnemyChase>() != null)
                {
                    Vector3 away = transform.position - other.transform.position;
                    away.y = 0f;

                    moveDir += away.normalized * separationStrength;
                }
            }

            moveDir.Normalize();
            controller.Move(moveDir * moveSpeed * Time.deltaTime);
        }
    }
}