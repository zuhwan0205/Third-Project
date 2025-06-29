using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public static WeaponController Instance;
    public WeaponType currentWeaponType;
    public Weapon currentWeapon;

    public Weapon[] weaponPrefabs;
    public bool[] ownedWeapons;
    public Transform weaponPos;

    private int currentWeaponIdx;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        currentWeaponType = WeaponType.None;

        ownedWeapons = new bool[5];
        currentWeaponIdx = -1;
    }

    public void EquipWeaponByIndex(int idx)
    {
        if (weaponPrefabs == null || weaponPrefabs.Length <= idx || weaponPos == null)
            return;

        // 같은 무기의 번호를 누르면 장비해제
        if (currentWeaponIdx == idx) 
        {
            ReturnToPool();
            currentWeaponIdx = -1;
            currentWeaponType = WeaponType.None;

            WeaponUIManager.Instance.UpdateWeaponUI(WeaponType.None);
            return;
        }
        
        // 무기 반환
        if (currentWeapon != null)
            ReturnToPool();
        
        // 무기 생성
        PoolKey poolKey = weaponPrefabs[idx].PoolKey;
        if (ObjectPoolManager.Instance.TryGetObject<Weapon>(poolKey, out var weaponObj))
        {
            weaponObj.transform.parent = weaponPos;
            weaponObj.transform.localPosition = weaponObj.InitialPosition;
            weaponObj.transform.localRotation = Quaternion.identity;
            SetWeapon(weaponObj);
            
            WeaponUIManager.Instance.UpdateWeaponUI(currentWeaponType);
        }
        currentWeaponIdx = idx;
    }

    private void SetWeapon(Weapon _weapon)
    {
        currentWeapon = _weapon;
        currentWeaponType = _weapon.WeaponType;
    }

    public void ReturnToPool()
    {
        ObjectPoolManager.Instance.ReturnObject(currentWeapon.PoolKey, currentWeapon.gameObject);
    }

    public void Attack()
    {
        if (currentWeapon == null) return;
        
        // 탄약이 필요한 무기인지 확인
        bool canAttack = true;
        
        if (currentWeaponType == WeaponType.Pistol)
        {
            // 총알 개수 확인
            if (InventoryManager.Instance.CheckItemCount("총알") > 0)
            {
                // 공격 후 총알 소모
                currentWeapon.Attack();
                InventoryManager.Instance.RemoveItem("총알");
            }
            else
            {
                Debug.Log("총알이 부족합니다!");
                canAttack = false;
            }
        }
        else if (currentWeaponType == WeaponType.Shotgun)
        {
            // 샷건 총알 개수 확인
            if (InventoryManager.Instance.CheckItemCount("샷건총알") > 0)
            {
                // 공격 후 샷건 총알 소모
                currentWeapon.Attack();
                InventoryManager.Instance.RemoveItem("샷건총알");
            }
            else
            {
                Debug.Log("샷건 총알이 부족합니다!");
                canAttack = false;
            }
        }
        else if (currentWeaponType == WeaponType.Bow)
        {
            // 화살 개수 확인
            if (InventoryManager.Instance.CheckItemCount("화살") > 0)
            {
                // 공격 후 화살 소모
                currentWeapon.Attack();
                InventoryManager.Instance.RemoveItem("화살");
            }
            else
            {
                Debug.Log("화살이 부족합니다!");
                canAttack = false;
            }
        }
        else
        {
            // 근접 무기 (도끼, 칼)는 탄약 필요 없음
            currentWeapon.Attack();
        }
    }

    public void Aim()
    {
        if (currentWeapon is Bow bow)
            bow.Aim();
    }

    public void AimCancel()
    {
        if (currentWeapon is Bow bow)
            bow.CancelAim();
    }

    public void Move(bool flag)
    {
        if (currentWeapon == null) return;
        currentWeapon.Move(flag);
    }

    public void Sprint(bool flag) => currentWeapon?.Sprint(flag);

    public void Reload()
    {
        if (currentWeapon is RangeWeapon rangeWeapon)
            rangeWeapon.Reload();
    }
}
