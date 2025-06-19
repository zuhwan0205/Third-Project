using UnityEngine;

public class ShortSword : MeleeWeapon
{
    public override void Attack()
    {
        if (attackRate > attackTime) return;
        attackTime = 0;
        PlaySwingAnimation();
        MeleeHitCheck();
    }
}
