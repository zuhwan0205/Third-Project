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

    private void Start()
    {
        Instance = this;
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
            
            // UI 업데이트 - 무기 없음 KYW2줄추가
            if (WeaponUIManager.Instance != null)
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
            
            // UI 업데이트 - 새로운 무기 KYW2줄추가
            if (WeaponUIManager.Instance != null)
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

    //KYW 수정 탄약감소 추가
    public void Attack()
    {
        currentWeapon?.Attack();
        if (currentWeapon.WeaponType == WeaponType.Pistol)
        {
            InventoryManager.Instance.RemoveItem("총알");
        }
        else if (currentWeapon.WeaponType == WeaponType.Bow)
        {
            InventoryManager.Instance.RemoveItem("화살");
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
