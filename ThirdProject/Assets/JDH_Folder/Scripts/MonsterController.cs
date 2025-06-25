using UnityEngine;

public class MonsterController : MonoBehaviour
{
    public float moveSpeed = 2f;              // 이동 속도
    public float stopDistance = 1.2f;         // 멈추는 거리
    public float attackDistance = 1.2f;       // 공격 거리
    public float attackCooldown = 2f;         // 공격 쿨타임

    public int maxHp = 100;                   // 최대 체력
    private int currentHp;                    // 현재 체력

    private Transform target;                 // 타겟 (플레이어)
    private Animator animator;                // 애니메이터
    private float lastAttackTime = 0f;        // 마지막 공격 시간
    private bool isAttacking = false;         // 공격 중 여부
    private bool isHit = false;               // 맞는 중 여부
    private bool isDead = false;              // 죽었는지 여부

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
        // 죽었거나, 타겟 없거나, 피격 중이거나 공격 중이면 행동 금지
        if (target == null || isAttacking || isHit || isDead) return;

        float distance = Vector3.Distance(transform.position, target.position);

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

    // 애니메이션 이벤트에서 호출: 공격 끝
    public void EndAttack()
    {
        isAttacking = false;
    }

    // 외부에서 데미지 입힘
    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHp -= damage;

        if (currentHp > 0)
        {
            animator?.SetTrigger("Hit");
            isHit = true;
        }
        else
        {
            Die();
        }
    }

    // 현재 체력 반환 (UI용)
    public int GetCurrentHealth() => currentHp;

    void Die()
    {
        if (isDead) return;

        StopMoving();
        isDead = true;
        animator?.SetTrigger("Die");

        // 죽음 애니메이션 마지막 프레임에 이벤트로 DestroySelf() 호출 가능
        Destroy(gameObject, 2f); // 예비 처리
    }

    // 애니메이션 이벤트에서 호출
    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    // 애니메이션 이벤트에서 호출
    public void EndHit()
    {
        isHit = false;
    }
}
