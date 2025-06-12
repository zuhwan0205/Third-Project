using UnityEngine;

public class Axe : MeleeWeapon
{
    private void OnEnable()
    {
        animator = GetComponent<Animator>();
    }
    public override void Attack()
    {
        Debug.Log("도끼 공격!");
        PlaySwingAnimation();
        MeleeHitCheck();
    }
}
