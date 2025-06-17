using UnityEngine;

public class MonsterController : MonoBehaviour
{
    public float moveSpeed = 2f;              // �̵� �ӵ�
    public float stopDistance = 1.2f;         // ���ߴ� �Ÿ�
    public float attackDistance = 1.2f;       // ���� �Ÿ�
    public float attackCooldown = 2f;         // ���� ��Ÿ��

    private Transform target;                 // Ÿ��
    private Animator animator;                // �ִϸ�����
    private float lastAttackTime = 0f;        // ������ ���� �ð�

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
        StopMoving(); // ���� ���¿����� ����

        if (Time.time - lastAttackTime >= attackCooldown)
        {
            if (animator != null)
            {
                animator.SetTrigger("Attack"); // Ʈ���� �� ���� �ߵ�
            }

            lastAttackTime = Time.time;

            // �������� �ִϸ��̼� �̺�Ʈ���� ó�� ��õ
        }
    }
}
