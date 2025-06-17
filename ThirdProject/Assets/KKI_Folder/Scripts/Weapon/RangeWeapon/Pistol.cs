using UnityEngine;

public class Pistol : RangeWeapon
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;

    public override void Attack()
    {
        if (isReloading) return;

        if (fireRate > fireTime) return;
        fireTime = 0f;

        if (currentAmmo > 0)
        {
            PlayFire();
            FireProjectile(bulletPrefab, firePoint, Quaternion.identity);
            currentAmmo--;
        }
        else
        {
            Reload();
        }
    }

    public override void Reload()
    {
        if (isReloading) return;
        
        if (reloadRate > reloadTime) return; 
        reloadTime = 0f;

        currentAmmo = maxAmmo;
        isReloading = true;
        PlayReload();
    }

}
