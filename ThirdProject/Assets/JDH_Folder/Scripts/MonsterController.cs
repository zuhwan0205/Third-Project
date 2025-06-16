using UnityEngine;

public class MonsterController : MonoBehaviour
{
    public float moveSpeed = 2f;              // 이동 속도
    public float stopDistance = 1.5f;         // 멈추는 거리
    private Transform target;                 // 타겟
    private Animator animator;                // 애니메이터 컴포넌트

    void Start()
    {
        // "Player" 태그를 가진 오브젝트를 타겟으로 설정
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
            target = player.transform;

        // Animator 컴포넌트를 가져옴
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (target == null) return;

        float distance = Vector3.Distance(transform.position, target.position);

        if (distance > stopDistance)
        {
            // 이동 방향 계산
            Vector3 direction = (target.position - transform.position).normalized;

            // 위치 이동
            transform.position += direction * moveSpeed * Time.deltaTime;

            // 회전 처리 (부드럽게)
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

            // 걷는 애니메이션 true 설정
            if (animator != null)
                animator.SetBool("isWalking", true);
        }
        else
        {
            // 멈춘 상태이므로 걷는 애니메이션 false 설정
            if (animator != null)
                animator.SetBool("isWalking", false);
        }
    }
}
