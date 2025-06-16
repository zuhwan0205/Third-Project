using System.Collections;
using UnityEngine;

public class Shotgun : RangeWeapon
{
    [SerializeField] private GameObject pelletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private int pelletCount = 6;
    [SerializeField] private float spreadAngle = 8f;
    private bool isReloading = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public override void Attack()
    {
        if (isReloading) return;

        if (currentAmmo > 0)
        {
            for (int i = 0; i < pelletCount; i++)
            {
                float angle = Random.Range(-spreadAngle, spreadAngle);
                Quaternion spreadRot = firePoint.rotation * Quaternion.Euler(0, angle, 0);
                FireProjectile(pelletPrefab, firePoint); // 퍼짐 로직, projectile 방향 반영하면 더 좋음
            }
            PlayFireAnimation();
            currentAmmo--;
        }
        else
        {
            Reload();
        }
    }

    public override void Reload()
    {
        if (!isReloading && currentAmmo < maxAmmo)
            StartCoroutine(ReloadCoroutine());
    }

    private IEnumerator ReloadCoroutine()
    {
        isReloading = true;
        while (currentAmmo < maxAmmo)
        {
            Debug.Log("한 발 장전!");
            currentAmmo++;
            PlayReloadAnimation();
            yield return new WaitForSeconds(reloadTime);

            if (Input.GetMouseButtonDown(0)) // 공격 시 재장전 중단
            {
                Debug.Log("재장전 취소!");
                break;
            }
        }
        isReloading = false;
    }
}
