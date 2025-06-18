using System.Collections;
using UnityEngine;
public class Bow : RangeWeapon
{
    public bool bArrow { get; private set;}
    public bool bAim {get; private set;}

    private void OnEnable()
    {
        animator.SetBool(AnimParams.B_ARROW, bArrow);
        animator.SetBool(AnimParams.B_AIM, bAim);
    }

    #region 공격
    public void Fire() {
        if (!bAim || !bArrow) return;
        Attack();
    }
    public override void Attack()
    {
        if (!bArrow || !bAim) return;

        if (fireRate > fireTime) return;
        fireTime = 0f;

        if (isReloading) return;

        PlayFire();
        // FireProjectile(firePoint, 1, 0f, PoolKey.Arrow);
        EndFire();
    }

    protected override void EndFire()
    {
        base.EndFire();
        bArrow = false;
        animator.SetBool(AnimParams.B_ARROW, bArrow);
        bAim = false;
        animator.SetBool(AnimParams.B_AIM, false);
    }
    #endregion

    #region 재장전
    public override void Reload()
    {
        if (!CanReloading()) return;
        if (bArrow) return;
        PlayReload();
    }

    protected override IEnumerator Reloading()
    {
        yield return new WaitForSeconds(reloadRate);
        EndReload();
    }

    protected override void EndReload()
    {
        isReloading = false;
        currentAmmo ++;
        reserveAmmo --;
        bArrow = true;
        animator.SetBool(AnimParams.B_ARROW, bArrow);
    }
    #endregion

    #region 에임
    public void Aim()
    {
        if (bAim) return;
        bAim = true;
        animator.SetBool(AnimParams.B_AIM, true);
    }

    public void CancelAim()
    {
        if (!bAim) return;
        bAim = false;
        animator.SetBool(AnimParams.B_AIM, false);
    }
    #endregion
    

}
