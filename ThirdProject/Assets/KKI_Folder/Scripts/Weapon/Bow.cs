using UnityEngine;

public class Bow : RangeWeapon
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;

    private void OnEnable()
    {
        animator = GetComponent<Animator>();
    }

    public override void Attack()
    {

    }

}
