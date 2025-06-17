using UnityEngine;

public class Axe : MeleeWeapon
{
    public override void Attack()
    {
        if (attackRate > attackTime) return;
        attackTime = 0;
        PlaySwingAnimation();
        MeleeHitCheck();
    }
}
