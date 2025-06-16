using System.Collections;
using UnityEngine;
public class Bow : RangeWeapon
{
    [SerializeField] GameObject arrowPrefab;
    [SerializeField] Transform firePoint;
    [SerializeField] float chargeTime = 1f;
    bool bArrow = false;
    bool bAim = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    private void OnEnable()
    {
        animator.SetBool("bArrow", bArrow);
        animator.ResetTrigger("Fire");
        animator.ResetTrigger("Reload");
    }

    public override void Attack()
    {
        if (!bArrow) return;
        Fire();
        bArrow = false;
        animator.SetBool("bArrow", bArrow);
    }

    public override void Reload()
    {
        if (bArrow) return;
        animator.SetTrigger("Reload");
        bArrow = true;
        animator.SetBool("bArrow", bArrow);
    }

    public void Aim()
    {
        if (bAim) return;
        bAim = true;
        animator.SetBool("bAim", true);
    }

    public void CancelAim()
    {
        if (bAim == false) return;
        bAim = false;
        animator.SetBool("bAim", false);
    }

    public void Fire() {
        if (!bAim || !bArrow) return;
        animator.SetTrigger("Fire");
        bArrow = false;
        animator.SetBool("bArrow", false);
        bAim = false;
    }

}
