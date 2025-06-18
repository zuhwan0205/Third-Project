using System.Collections;
using UnityEngine;


public abstract class RangeWeapon : Weapon
{
    [SerializeField] protected int maxAmmo;
    [SerializeField] protected int currentAmmo;
    [SerializeField] protected int reserveAmmo;
    [SerializeField] protected float fireRate;
    [SerializeField] protected float reloadRate;
    [SerializeField] protected Transform firePoint;
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

    #region Fire(발사)
    protected virtual void PlayFire() 
    {
        // 애니메이션 재생
        animator?.Play(AnimParams.FIRE);

        // 사운드 재생(사격음)
        if (audioSource != null && fireSfx != null)
            audioSource.PlayOneShot(fireSfx);
    }
    protected void FireProjectile(Transform firePoint, int projectileCount, float spreadAngle, PoolKey poolKey)
    {
        Camera cam = Camera.main;
        Vector3 fireOrigin = firePoint.position;

        // 기준 방향 : 크로스헤어 중심
        Vector3 baseDir;
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f))
            baseDir = (hit.point - fireOrigin).normalized;
        else
            baseDir = cam.transform.forward;

        for (int i = 0; i < projectileCount; i++)
        {
            Vector3 dir = baseDir;

            if (spreadAngle > 0f)
            {
                /* insideUnitCircle 설명
                Random.insideUnitCircle
                    (0,0) 중심의 반지름 1짜리 원 안에서 임의의 한 점을 리턴
                    예: (0.12, -0.3), (-0.98, 0.42) 등
                    → spread.x, spread.y로 각각 yaw(좌우), pitch(상하) 각도를 뽑을 수 있음
                */
                Vector2 spread = Random.insideUnitCircle * spreadAngle;
                /* Euler 설명 
                Quaternion.Euler(상하, 좌우, 롤)
                    오일러 각도(피치, 요, 롤) → 쿼터니언으로 변환

                    여기서 spread.y = pitch(상하로 위아래로 살짝),
                    spread.x = yaw(좌우로 살짝)
                */
                Quaternion spreadRot = Quaternion.Euler(spread.y, spread.x, 0);
                dir = spreadRot * baseDir;
            }

            if (ObjectPoolManager.Instance.TryGetObject<Bullet>(poolKey, out var projectile))
            {
                projectile.OnSpawn(firePoint, dir);
            }
        }
    }

    protected virtual void EndFire()
    {
        currentAmmo --;
    }
    #endregion

    #region 재장전
    public abstract void Reload();

    protected bool CanReloading()
    {
        if (isReloading) return false;
        if (currentAmmo >= maxAmmo) return false;
        if (reserveAmmo <= 0) return false;
        if (reloadRate > reloadTime) return false;

        return true;
    }
    protected virtual void PlayReload() 
    {
        reloadTime = 0f;
        isReloading = true;
        
        // 애니메이션 재생
        animator?.Play(AnimParams.RELOAD);

        // 사운드 재생(장전음)
        if (audioSource != null && reloadSfx != null)
            audioSource.PlayOneShot(reloadSfx);

        StartCoroutine(Reloading());
    } 

    protected abstract IEnumerator Reloading();

    protected abstract void EndReload();
    #endregion
}