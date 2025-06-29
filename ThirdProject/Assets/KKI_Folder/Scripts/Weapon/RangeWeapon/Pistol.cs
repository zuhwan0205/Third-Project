using System.Collections;
using JetBrains.Annotations;
using UnityEngine;

public class Pistol : RangeWeapon
{
    
    void Start()
    {    
        reserveAmmo = InventoryManager.Instance.CheckItemCount("총알");
    }

    #region 공격
    public override void Attack()
    {
        if (isReloading) return;

        if (fireRate > fireTime) return;
        fireTime = 0f;

        if (currentAmmo > 0)
        {
            PlayFire();
            FireProjectile(firePoint, 1, 0f, PoolKey.Bullet);
            EndFire();
            WeaponUIManager.Instance.UpdateWeaponUI(WeaponType.Pistol, currentAmmo);
        }
        else
        {
            Reload();
        }
    }
    #endregion

    #region 재장전
    public override void Reload()
    {
        if (!CanReloading()) return;
        PlayReload();

        return ;
    }


    protected override IEnumerator Reloading()
    {
        yield return new WaitForSeconds(reloadRate);
        EndReload();
    }

    protected override void EndReload()
    {
        int needed = maxAmmo - currentAmmo;
        int toLoad = Mathf.Min(needed, reserveAmmo);

        currentAmmo += toLoad;
        reserveAmmo -= toLoad;
        InventoryManager.Instance.RemoveItem("총알", toLoad);
        WeaponUIManager.Instance.UpdateWeaponUI(WeaponType.Pistol, toLoad);

        isReloading = false;
    }
    #endregion
}
