using System.Collections;
using UnityEngine;

public class Shotgun : RangeWeapon
{
    [SerializeField] private GameObject pelletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private int pelletCount = 6;
    [SerializeField] private float spreadAngle = 8f;
    [SerializeField] private float pelletReloadTime;

    public override void Attack()
    {
        if (fireRate > fireTime) return;
        fireTime = 0f;

        if (isReloading) return;

        if (currentAmmo > 0)
        {
            for (int i = 0; i < pelletCount; i++)
            {
                float angle = Random.Range(-spreadAngle, spreadAngle);
                Quaternion spreadRot = firePoint.rotation * Quaternion.Euler(0, angle, 0);
                FireProjectile(pelletPrefab, firePoint, spreadRot); // 퍼짐 로직, projectile 방향 반영하면 더 좋음
            }
            PlayFire();
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

        isReloading = true;
        currentAmmo++;
        PlayReload();
    }
}
