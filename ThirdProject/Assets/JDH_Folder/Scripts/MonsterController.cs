using UnityEngine;

public class MonsterController : MonoBehaviour
{
    public float moveSpeed = 2f;              // �̵� �ӵ�
    public float stopDistance = 1.5f;         // ���ߴ� �Ÿ�
    private Transform target;                 // Ÿ��
    private Animator animator;                // �ִϸ����� ������Ʈ

    void Start()
    {
        // "Player" �±׸� ���� ������Ʈ�� Ÿ������ ����
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
            target = player.transform;

        // Animator ������Ʈ�� ������
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (target == null) return;

        float distance = Vector3.Distance(transform.position, target.position);

        if (distance > stopDistance)
        {
            // �̵� ���� ���
            Vector3 direction = (target.position - transform.position).normalized;

            // ��ġ �̵�
            transform.position += direction * moveSpeed * Time.deltaTime;

            // ȸ�� ó�� (�ε巴��)
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

            // �ȴ� �ִϸ��̼� true ����
            if (animator != null)
                animator.SetBool("isWalking", true);
        }
        else
        {
            // ���� �����̹Ƿ� �ȴ� �ִϸ��̼� false ����
            if (animator != null)
                animator.SetBool("isWalking", false);
        }
    }
}
