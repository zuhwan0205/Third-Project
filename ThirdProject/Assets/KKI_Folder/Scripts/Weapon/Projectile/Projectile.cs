using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("풀 세팅")]
    [Tooltip("ObjectPoolManager의 PoolSettings.Key와 동일하게 설정")]
    [SerializeField] private PoolKey poolKey;
    
    [Header("투사체 기본 세팅")]
    [SerializeField] protected float damage = 25f;
    [SerializeField] protected float speed = 20f;
    [SerializeField] protected float lifeTime = 5f;

    protected Rigidbody rb;
    private float timer;

    private Vector3 lastPosition;

    protected virtual void OnEnable() 
    {
        lastPosition = transform.position;
    }

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public virtual void OnSpawn(Transform _transform, Vector3 direction)
    {
        gameObject.transform.position = _transform.position;
        transform.forward = direction;
        rb.linearVelocity = direction * speed;
        gameObject.SetActive(true);
    }

    protected virtual void Update()
    {
        timer += Time.deltaTime;
        if (timer >= lifeTime)
            ReturnToPool();

        lastPosition = transform.position;
    }

    protected virtual void OnCollisionEnter(Collision other)
    {
        Debug.Log("ReturnToPool!");
        ReturnToPool();
    }

    protected void ReturnToPool()
    {
        ObjectPoolManager.Instance.ReturnObject(poolKey, gameObject);
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
