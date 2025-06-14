using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public string weaponName;
    public WeaponType weaponType;
    protected Animator animator;
    public abstract void Attack();
    public void Move(bool flag)
    {
        animator.SetBool("Walk", flag);
    }
    public void Sprint(bool flag)
    {
        animator.SetBool("Run", flag);
    }
}

