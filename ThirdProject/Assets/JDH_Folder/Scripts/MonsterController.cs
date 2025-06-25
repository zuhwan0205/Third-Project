using UnityEngine;

public class MonsterController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 2f;
    public float stopDistance = 1.2f;
    public float attackDistance = 1.2f;
    public float attackCooldown = 2f;

    [Header("Health")]
    public int maxHp = 100;
    private int currentHp;

    private Transform target;
    private Animator animator;
    private float lastAttackTime = 0f;

    private bool isAttacking = false;
    private bool isHit = false;
    private bool isDead = false;

    void Start()
    {
        currentHp = maxHp;

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
            target = player.transform;

        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (target == null || isDead || isHit || isAttacking) return;

        float distance = Vector3.Distance(
            new Vector3(transform.position.x, 0, transform.position.z),
            new Vector3(target.position.x, 0, target.position.z)
        );

        if (distance > stopDistance)
        {
            MoveToTarget();
        }
        else if (distance <= attackDistance)
        {
            Attack();
        }
        else
        {
            StopMoving();
        }
    }

    void MoveToTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;

        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

        animator?.SetBool("isWalking", true);
    }

    void StopMoving()
    {
        animator?.SetBool("isWalking", false);
    }

    void Attack()
    {
        StopMoving();

        if (Time.time - lastAttackTime >= attackCooldown)
        {
            animator?.SetTrigger("Attack");
            isAttacking = true;
            lastAttackTime = Time.time;
        }
    }

    public void DealDamage()
    {
        if (target == null) return;

        float dist = Vector3.Distance(
            new Vector3(transform.position.x, 0, transform.position.z),
            new Vector3(target.position.x, 0, target.position.z)
        );

        if (dist <= attackDistance)
        {
            PlayerHealth playerHealth = target.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(10); // 원하는 데미지 수치
            }
        }
    }

    public void EndAttack()
    {
        isAttacking = false;
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHp -= damage;
        animator?.SetTrigger("Hit");
        isHit = true;

        if (currentHp <= 0)
        {
            Die();
        }
    }

    public void EndHit()
    {
        isHit = false;
    }

    void Die()
    {
        isDead = true;
        StopMoving();
        animator?.SetTrigger("Die");
        Destroy(gameObject, 2f);
    }

    public int GetCurrentHealth()
    {
        return currentHp;
    }
}
