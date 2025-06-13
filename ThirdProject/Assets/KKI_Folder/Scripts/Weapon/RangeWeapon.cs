using UnityEngine;


public abstract class RangeWeapon : Weapon
{
    [SerializeField] protected int maxAmmo;
    [SerializeField] protected int currentAmmo;
    [SerializeField] protected float reloadTime;

    // 원거리무기만의 공통 메서드 예시
    protected void PlayFireAnimation()
    {
        animator.Play("Fire");
    }
    protected void FireProjectile(GameObject prefab, Transform firePoint)
    {
        Instantiate(prefab, firePoint.position, firePoint.rotation);
        Debug.Log($"{weaponName} 발사!");
    }

}