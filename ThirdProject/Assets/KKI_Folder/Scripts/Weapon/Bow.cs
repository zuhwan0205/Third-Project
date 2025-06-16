using System.Collections;
using UnityEngine;
public class Bow : RangeWeapon
{
    [SerializeField] GameObject arrowPrefab;
    [SerializeField] Transform firePoint;
    bool bArrow = false;
    bool bAim = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    private void OnEnable()
    {
        animator.SetBool("bArrow", bArrow);
        animator.ResetTrigger(AnimParams.FIRE);
        animator.ResetTrigger(AnimParams.RELOAD);
    }

    public override void Attack()
    {
        if (!bArrow) return;
        Fire();
        bArrow = false;
        animator.SetBool(AnimParams.B_ARROW, bArrow);
    }

    public override void Reload()
    {
        if (bArrow) return;
        animator.SetTrigger(AnimParams.FIRE);
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
        animator.SetTrigger(AnimParams.FIRE);
        bArrow = false;
        animator.SetBool(AnimParams.B_ARROW, false);
        bAim = false;
    }

}
