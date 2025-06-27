using UnityEngine;
using Fusion;

public class Projectile : NetworkBehaviour
{
    [Header("풀 세팅")]
    [SerializeField] private PoolKey poolKey;
    
    [Header("투사체 기본 세팅")]
    [SerializeField] protected int damage = 25;
    [SerializeField] protected float speed = 20f;
    [SerializeField] protected float lifeTime = 5f;

    protected Rigidbody rb;
    private float timer;

    private Vector3 lastPosition;
    private Vector3 direction;


    public void Init(Vector3 dir)
    {
        direction = dir;
        timer = 0f;
        if (rb != null)
            rb.linearVelocity = direction * speed;
        else 
            Debug.LogWarning("Projectile에 RigidBody가 없음.");
    }

    public override void Spawned()
    {
        lastPosition = transform.position;
        timer = 0f;
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority) return;

        // 이동
        if (rb != null)
            rb.linearVelocity = direction * speed;
        else
            transform.position += direction * speed * Runner.DeltaTime;

        timer += Runner.DeltaTime;
        if (timer >= lifeTime)
        {
            Runner.Despawn(Object); // NetworkObject 디스폰
        }

        lastPosition = transform.position;
    }

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    protected virtual void OnCollisionEnter(Collision other)
    {
        if (!Object.HasStateAuthority) return;
        
        var target = other.collider.GetComponent<MonsterController>();
        if (target != null)
        {
            // 데미지 처리
            target.TakeDamage(damage);
            Debug.Log("몬스터 데미지 처리!");
        }
        else
            Debug.Log("몬스터 안 맞음!");
    
        Runner.Despawn(Object);
    }
    
    private void OnDrawGizmos()
    {
        // 현재 위치~앞 방향으로 "예상 궤적" 표시
        Gizmos.color = Color.yellow;
        Vector3 start = transform.position;
        Vector3 direction = rb != null ? rb.linearVelocity.normalized : transform.forward;
        Gizmos.DrawLine(start, start + direction * 5f); // 5f=길이, 조절 가능
        Gizmos.DrawWireSphere(start, 0.1f);

        // (선택) Trail처럼 "이전 위치~현재 위치"도 선으로 표시
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(lastPosition, transform.position);
    }


}
