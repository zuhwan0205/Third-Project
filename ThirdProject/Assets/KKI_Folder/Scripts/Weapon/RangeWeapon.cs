using UnityEngine;


public abstract class RangeWeapon : Weapon
{
    [SerializeField] protected int maxAmmo;
    [SerializeField] protected int currentAmmo;
    [SerializeField] protected float reloadTime;

    protected virtual void PlayFireAnimation() => animator?.Play(AnimParams.FIRE);
    protected virtual void PlayReloadAnimation() => animator?.Play(AnimParams.RELOAD);

    protected virtual void FireProjectile(GameObject prefab, Transform firePoint)
    {
        Instantiate(prefab, firePoint.position, firePoint.rotation);
        Debug.Log($"{weaponName} 발사!");
    }

    public abstract void Reload();

}