using UnityEngine;

public class Pistol : RangeWeapon
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public override void Attack()
    {
        if (currentAmmo > 0)
        {
            PlayFireAnimation();
            FireProjectile(bulletPrefab, firePoint);
            currentAmmo--;
        }
        else
        {
            PlayReloadAnimation();
            Reload();
        }
    }

    public override void Reload()
    {
        currentAmmo = maxAmmo;
        PlayReloadAnimation();
    }

}
