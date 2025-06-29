using System.Collections;
using JetBrains.Annotations;
using UnityEngine;

public class Pistol : RangeWeapon
{
    void Start()
    {
        WeaponUIManager.Instance.UpdateWeaponUI(WeaponType.Pistol, currentAmmo);
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
        if (!CanReloading())    
            {Debug.Log("리러딩 실패"); return;  }
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
        for (int i = 0; i < toLoad; i ++)
            InventoryManager.Instance.RemoveItem("총알");
        WeaponUIManager.Instance.UpdateWeaponUI(WeaponType.Pistol, currentAmmo);

        isReloading = false;
    }
    #endregion
}
