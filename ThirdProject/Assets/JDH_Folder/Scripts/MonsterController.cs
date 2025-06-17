using UnityEngine;

public class MonsterController : MonoBehaviour
{
    public float moveSpeed = 2f;              // 이동 속도
    public float stopDistance = 1.2f;         // 멈추는 거리
    public float attackDistance = 1.2f;       // 공격 거리
    public float attackCooldown = 2f;         // 공격 쿨타임

    private Transform target;                 // 타겟
    private Animator animator;                // 애니메이터
    private float lastAttackTime = 0f;        // 마지막 공격 시간

    void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
            target = player.transform;

        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (target == null) return;

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

        if (animator != null)
        {
            animator.SetBool("isWalking", true);
        }
    }

    void StopMoving()
    {
        if (animator != null)
        {
            animator.SetBool("isWalking", false);
        }
    }

    void Attack()
    {
        StopMoving(); // 정지 상태에서만 공격

        if (Time.time - lastAttackTime >= attackCooldown)
        {
            if (animator != null)
            {
                animator.SetTrigger("Attack"); // 트리거 한 번만 발동
            }

            lastAttackTime = Time.time;

            // 데미지는 애니메이션 이벤트에서 처리 추천
        }
    }
}
