using UnityEngine;


public enum WeaponType
{
    None,
    Axe,
    Sword,
    Bow,
    Pistol,
    Shotgun
}


public abstract class Weapon : MonoBehaviour
{
    public string weaponName;
    public WeaponType weaponType;

    protected Animator animator;
    public abstract void Attack();
}

public abstract class MeleeWeapon : Weapon
{
    [SerializeField] protected float damage;            // 데미지
    [SerializeField] protected float attackRate;        // 공격 속도
    [SerializeField] protected float attackRange;       // 판정 구의 반지름
    [SerializeField] protected LayerMask enemyLayer;    // 적 레이어 판정
    [SerializeField] protected AudioClip swingSound;    // 공격 소리
    

    protected void PlaySwingAnimation()
    {
        Debug.Log($"{weaponName} 휘두르기");

        // 애니메이션 넣기

        animator.SetBool("Attack",true);
    }

    protected void MeleeHitCheck()
    {
        // 플레이어 위치 또는 무기 앞 위치 기준으로 구 판정
        Vector3 hitOrigin = transform.position + transform.forward * 1f; // 플레이어 위치
        Collider[] hitEnemies = Physics.OverlapSphere(hitOrigin, attackRange, enemyLayer);
        
        foreach(Collider enemy in hitEnemies)
        {
            Debug.Log(enemy.name + "에게 공격");

            // 적 공격 처리
        }

        DebugDrawHitArea(hitOrigin);
    }

    // 씬에서 판정 영역 보이게 함
    protected void DebugDrawHitArea(Vector3 pos)
    {
        Debug.DrawLine(transform.position, pos, Color.yellow, 0.5f);
    }

    // 에디터에서 구체가 보이게 함
    protected void OnDrawGizmosSelected() 
    {
        Vector3 hitOrigin = transform.position + transform.forward * 1f;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(hitOrigin, attackRange);
    }
}


public abstract class RangeWeapon : Weapon
{
    [SerializeField] protected int maxAmmo;
    [SerializeField] protected int currentAmmo;
    [SerializeField] protected float reloadTime;

    // 원거리무기만의 공통 메서드 예시
    protected void FireProjectile(GameObject prefab, Transform firePoint)
    {
        Instantiate(prefab, firePoint.position, firePoint.rotation);
        Debug.Log($"{weaponName} 발사!");
    }

    protected void Reload()
    {
        Debug.Log($"{weaponName} 재장전");
        currentAmmo = maxAmmo;
    }
}