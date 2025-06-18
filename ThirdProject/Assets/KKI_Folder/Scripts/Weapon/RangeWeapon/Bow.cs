using System.Collections;
using UnityEngine;
public class Bow : RangeWeapon
{
    [SerializeField] GameObject arrowPrefab;
    [SerializeField] Transform firePoint;
    bool bArrow = false;
    bool bAim = false;

    private void OnEnable()
    {
        animator.SetBool("bArrow", bArrow);
    }

    public override void Attack()
    {
        if (!bArrow || !bAim) return;

        if (fireRate > fireTime) return;
        fireTime = 0f;

        if (isReloading) return;

        PlayFire();
        bArrow = false;
        animator.SetBool(AnimParams.B_ARROW, bArrow);
        bAim = false;
        animator.SetBool(AnimParams.B_AIM, false);
    }

    public override void Reload()
    {
        if (reloadRate > reloadTime) return;
        reloadTime = 0f;

        if (bArrow) return;
        isReloading = true;
        PlayReload();
        bArrow = true;
        animator.SetBool(AnimParams.B_ARROW, bArrow);
    }

    public void Aim()
    {
        if (bAim) return;
        bAim = true;
        animator.SetBool(AnimParams.B_AIM, true);
    }

    public void CancelAim()
    {
        if (bAim == false) return;
        bAim = false;
        animator.SetBool(AnimParams.B_AIM, false);
    }

    public void Fire() {
        if (!bAim || !bArrow) return;
        Attack();
    }

}
