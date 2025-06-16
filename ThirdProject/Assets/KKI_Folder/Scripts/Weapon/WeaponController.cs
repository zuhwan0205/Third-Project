using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public WeaponType currentWeaponType;
    public Weapon currentWeapon;

    public Weapon[] weaponPrefabs;
    public Transform weaponPos;

    private void Start()
    {
        currentWeaponType = WeaponType.None;
    }

    private void Update()
    {
        // 디버그용 테스트
        if (Input.GetKeyDown(KeyCode.Comma))    EquipWeaponByIndex(0);
        if (Input.GetKeyDown(KeyCode.Home))     EquipWeaponByIndex(1);
        if (Input.GetKeyDown(KeyCode.End))      EquipWeaponByIndex(2);
        if (Input.GetKeyDown(KeyCode.PageDown)) EquipWeaponByIndex(3);
    }

    private void EquipWeaponByIndex(int idx)
    {
        if (weaponPrefabs == null || weaponPrefabs.Length <= idx || weaponPos == null)
            return;

        if (currentWeapon != null)
            Destroy(currentWeapon.gameObject);

        Weapon weaponObj = Instantiate(weaponPrefabs[idx], weaponPos.transform);
        weaponObj.transform.localPosition = Vector3.zero;
        weaponObj.transform.localRotation = Quaternion.identity;
        SetWeapon(weaponObj);
    }

    public void SetWeapon(Weapon _weapon)
    {
        currentWeapon = _weapon;
        currentWeaponType = _weapon.weaponType;
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

    public void Move(Vector3 move)
    {
        if (currentWeapon == null) return;
        bool isMoving = move.sqrMagnitude > 0.001f;
        currentWeapon.Move(isMoving);
    }

    public void Sprint(bool flag) => currentWeapon?.Sprint(flag);

    public void Reload()
    {
        if (currentWeapon is RangeWeapon rangeWeapon)
            rangeWeapon.Reload();
    }
}
