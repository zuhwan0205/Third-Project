using UnityEngine;

public class Arrow : Projectile
{
    [Header("화살 전용 옵션")]
    [SerializeField] private AudioClip hitSound;

    // 만약 화살에만 적용할 변수 추가 가능

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    public override void OnSpawn(Transform _transform, Vector3 direction)
    {
        base.OnSpawn(_transform, direction);
    }

    protected override void OnCollisionEnter(Collision other)
    {
        // 사운드 재생
        if (hitSound)
        {
            AudioSource.PlayClipAtPoint(hitSound, transform.position);
        }

        // 2. 데미지 주기 및 추가로 화살에만 적용되는 효과(예: 몬스터에 꽂힌다든가)
        // MonsterController target = other.collider.GetComponent<MonsterController>();

        // 3. 부모의 반환 처리
        base.OnCollisionEnter(other);
    }
}
