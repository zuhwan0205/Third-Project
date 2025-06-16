using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public string weaponName;
    public WeaponType weaponType;
    protected Animator animator;

    public abstract void Attack();
    public virtual void Move(bool isMoving) { animator?.SetBool(AnimParams.WALK, isMoving); }
    public virtual void Sprint(bool isSprinting) { animator?.SetBool(AnimParams.RUN, isSprinting); }
}


public static class AnimParams
{
    public const string ATTACK = "Attack";
    public const string FIRE = "Fire";
    public const string RELOAD = "Reload";
    public const string B_ARROW = "bArrow";
    public const string B_AIM = "bAim";
    public const string WALK = "Walk";
    public const string RUN = "Run";
}