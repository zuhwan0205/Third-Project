using UnityEngine;

public class Axe : MeleeWeapon
{
    public override void Attack()
    {
        if (attackRate > attackTime) return;
        attackTime = 0;
        if (Object.HasInputAuthority)  // 연출은 클라이언트
            PlaySwingAnimation(); 

        if (Object.HasStateAuthority)  // 판정/데미지는 서버에서만
            MeleeHitCheck();
    }
}
