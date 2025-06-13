using UnityEngine;

public class Pistol : RangeWeapon
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;

    private void OnEnable()
    {
        animator = GetComponent<Animator>();
    }

    public override void Attack()
    {
        if (currentAmmo > 0)
        {
            PlayFireAnimation();
            FireProjectile(bulletPrefab, firePoint);
            currentAmmo --;

            // 사운드, 이펙트
        }
        else
        {
            PlayReloadAnimation();
            Reload();
        }
    }

    void PlayReloadAnimation()
    {
        animator.Play("Reload");
    }
    public void Reload()
    {
        // 현재 가지고 있는 탄약이 최대 탄약보다 많으면
        currentAmmo = maxAmmo;
        // 그게 아니라면 남은 소량만 넣기
    }

}
