using UnityEngine;
using UnityEngine.AI;

public class MonsterController : MonoBehaviour
{
    [Header("Movement")]
    public float stopDistance = 1.2f;
    public float attackDistance = 1.2f;
    public float attackCooldown = 2f;

    [Header("Health")]
    public int maxHp = 100;
    private int currentHp;

    private Transform target;
    private Animator animator;
    private NavMeshAgent agent;
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
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (target == null || isDead || isHit) return;

        float distance = Vector3.Distance(transform.position, target.position);

        if (!isAttacking && distance > stopDistance)
        {
            agent.isStopped = false;
            agent.SetDestination(target.position);
            animator?.SetBool("isWalking", true);
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

    void StopMoving()
    {
        agent.isStopped = true;
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

            // 회전해서 바라보게
            Vector3 dir = (target.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }

    public void DealDamage()
    {
        if (target == null) return;

        float dist = Vector3.Distance(transform.position, target.position);

        if (dist <= attackDistance)
        {
            PlayerHealth playerHealth = target.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(10);
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
