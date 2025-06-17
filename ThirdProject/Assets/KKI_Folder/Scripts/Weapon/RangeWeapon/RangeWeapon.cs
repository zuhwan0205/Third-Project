using System.Collections;
using UnityEngine;


public abstract class RangeWeapon : Weapon
{
    [SerializeField] protected int maxAmmo;
    [SerializeField] protected int currentAmmo;
    [SerializeField] protected float fireRate;
    [SerializeField] protected float reloadRate;
    [SerializeField] protected AudioClip fireSfx;
    [SerializeField] protected AudioClip reloadSfx;

    protected float fireTime;
    protected float reloadTime;
    protected bool isReloading;

    void Update()
    {
        fireTime += Time.deltaTime;
        reloadTime += Time.deltaTime;
    }

    protected virtual void PlayFire() 
    {
        // 애니메이션 재생
        animator?.Play(AnimParams.FIRE);

        // 사운드 재생(사격음)
        if (audioSource != null && fireSfx != null)
            audioSource.PlayOneShot(fireSfx);
    }

    protected virtual void PlayReload() 
    {
        StartCoroutine(Reloading());

        // 애니메이션 재생
        animator?.Play(AnimParams.RELOAD);

        // 사운드 재생(장전음)
        if (audioSource != null && reloadSfx != null)
            audioSource.PlayOneShot(reloadSfx);

    } 

    protected virtual void FireProjectile(GameObject prefab, Transform firePoint, Quaternion quaternion)
    {
        if (prefab == null || firePoint == null) return;
        Instantiate(prefab, firePoint.position, quaternion);
        Debug.Log($"{weaponName} 발사!");
    }

    protected IEnumerator Reloading()
    {
        yield return new WaitForSeconds(reloadRate);
        isReloading = false;
    }
    public abstract void Reload();

}