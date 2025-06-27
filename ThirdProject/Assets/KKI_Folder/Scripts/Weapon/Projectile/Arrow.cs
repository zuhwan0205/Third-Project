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

        //  부모 반환 처리
        base.OnCollisionEnter(other);
    }
}
