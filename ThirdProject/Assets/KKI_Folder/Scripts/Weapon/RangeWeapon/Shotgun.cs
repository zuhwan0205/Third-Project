using System.Collections;
using UnityEngine;

public class Shotgun : RangeWeapon
{
    [Header("샷건 개인 설정")]
    [SerializeField] private int pelletCount = 6;
    [SerializeField] private float spreadAngle = 8f;

    private Coroutine reloadCoroutine;

    #region 공격
    public override void Attack()
    {
        if (fireRate > fireTime) return;
        fireTime = 0f;

        if (currentAmmo > 0)
        {
            // 재장전 중일 때 쏠 수 있음
            if (isReloading)
            {
                if (reloadCoroutine != null)
                {
                    StopCoroutine(reloadCoroutine);
                    reloadCoroutine = null;
                }
                isReloading = false;
            }
            PlayFire();
            FireProjectile(firePoint, pelletCount, spreadAngle, PoolKey.ShotgunPellet);
            EndFire();
        }
        else
        {
            Reload();
        }
    }
    #endregion
    
    #region 재장전
    public override void Reload()
    {
        if (!CanReloading()) return;

        PlayReload();
    }

    protected override void PlayReload()
    {
        isReloading = true;
        reloadCoroutine = StartCoroutine(Reloading());
    }
    protected override IEnumerator Reloading()
    {
        while (currentAmmo < maxAmmo && reserveAmmo > 0)
        {
            // 애니메이션/사운드(한발 장전)
            animator?.Play(AnimParams.RELOAD);
            if (audioSource != null && reloadSfx != null)
                audioSource.PlayOneShot(reloadSfx);

            yield return new WaitForSeconds(reloadRate);

            currentAmmo++;
            reserveAmmo--;
        }

        EndReload();
    }

    protected override void EndReload()
    {
        isReloading = false;
        reloadCoroutine = null;
    }
    #endregion
    
}
