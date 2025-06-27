using UnityEngine;
using Fusion;

public class WeaponController : NetworkBehaviour
{
    public static WeaponController Instance;
    public WeaponType currentWeaponType;
    public Weapon currentWeapon;

    [SerializeField] private Weapon[] weaponPrefabs;
    public bool[] ownedWeapons;
    public Transform weaponPos;

    [Networked] private int equippedWeaponIdx { get; set; }  // 서버 기준 무기 인덱스

    private void Awake()
    {
        if (Instance != null && Instance == this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        currentWeaponType = WeaponType.None;
        ownedWeapons = new bool[5];
        equippedWeaponIdx = -1;
    }

    public void EquipWeaponByIndex(int idx)
    {
        // 서버에서만 실제 장착 로직
        if (!Object.HasStateAuthority) return;

        if (weaponPrefabs == null || weaponPrefabs.Length <= idx || weaponPos == null)
            return;

        // 같은 무기의 번호를 누르면 장비해제
        if (equippedWeaponIdx == idx) 
        {
            ReturnToPool();
            equippedWeaponIdx = -1;
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
        }
        equippedWeaponIdx = idx;
        currentWeaponType = weaponPrefabs[idx].WeaponType;
    }

    private void SetWeapon(Weapon _weapon)
    {
        currentWeapon = _weapon;
        currentWeaponType = _weapon.WeaponType;
    }

    public void ReturnToPool()
    {
        if (currentWeapon == null) return;
        ObjectPoolManager.Instance.ReturnObject(currentWeapon.PoolKey, currentWeapon.gameObject);
        currentWeapon = null;
        currentWeaponType = WeaponType.None;
    }

    public void Attack() => currentWeapon?.Attack();

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
