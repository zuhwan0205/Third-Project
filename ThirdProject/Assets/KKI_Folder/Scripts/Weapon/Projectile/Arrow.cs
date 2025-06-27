using UnityEngine;

public class Arrow : Projectile
{
    [Header("화살 전용 옵션")]
    [SerializeField] private AudioClip hitSound;

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
