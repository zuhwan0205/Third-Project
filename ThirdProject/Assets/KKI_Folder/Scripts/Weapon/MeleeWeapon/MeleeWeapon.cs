using UnityEngine;

public abstract class MeleeWeapon : Weapon
{
    [Header("공격 설정")]
    [SerializeField] protected float damage = 25f;              // 데미지
    [SerializeField] protected float attackRate = 1f;           // 공격 속도
    [SerializeField] protected float attackRange = 1.5f;        // 공격 반경
    [SerializeField] protected LayerMask enemyLayer;            // 공격 대상 레이어
    [SerializeField] protected AudioClip swingSfx;              // 휘두르는 소리

    [Header("공격 범위 중심 위치")]
    [SerializeField] protected Transform attackOrigin;          // 공격 중심 위치 (ex. 무기 끝 지점)

    protected float attackTime;

    void Update()
    {
        attackTime += Time.deltaTime;
    }

    protected void PlaySwingAnimation()
    {
        // 애니메이션 재생
        animator?.Play(AnimParams.ATTACK);

        // 소리 재생
        if (audioSource != null && swingSfx != null)
            audioSource.PlayOneShot(swingSfx);
    }

    protected void MeleeHitCheck()
    {
        Vector3 hitPos = GetHitOrigin();

        Collider[] hitEnemies = Physics.OverlapSphere(hitPos, attackRange, enemyLayer);
        foreach (Collider enemy in hitEnemies)
        {
            Debug.Log(enemy.name + "에게 공격");

            if (enemy.TryGetComponent<MonsterController>(out var monster))
            {
                monster.TakeDamage((int)damage);
            }
        }

        DebugDrawHitArea(hitPos);
    }

    protected Vector3 GetHitOrigin()
    {
        // 지정된 공격 중심 Transform이 있으면 그걸 사용, 없으면 기본 forward 기준
        return attackOrigin != null
            ? attackOrigin.position
            : transform.position + transform.forward * 1f;
    }
    protected void DebugDrawHitArea(Vector3 pos)
    {
        // 공격 중심 방향선 (forward 방향으로)
        Debug.DrawRay(pos, transform.forward * attackRange, Color.red, 0.5f);

        // 플레이어 기준에서 공격 중심까지 선
        Debug.DrawLine(transform.position, pos, Color.yellow, 0.5f);
    }
    // 에디터에서 Gizmo로 시각화 (선택 시)
    protected void OnDrawGizmosSelected()
    {
        Vector3 center = GetHitOrigin();

        // 와이어 구체 (빨간색): 공격 범위 표시
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(center, attackRange);

        // 채워진 반투명 구체 (선택): 더 명확한 시각화
        Gizmos.color = new Color(1f, 0f, 0f, 0.15f);
        Gizmos.DrawSphere(center, attackRange);
    }
}
