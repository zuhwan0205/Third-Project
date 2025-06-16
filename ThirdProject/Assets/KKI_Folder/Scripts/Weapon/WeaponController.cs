using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public WeaponType currentWeaponType;
    public Weapon currentWeapon;

    public Weapon[] weaponPrefabs;
    public Transform weaponPos;
    
    void Start()
    {
        currentWeaponType = WeaponType.None;   
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Comma))
        {
            Weapon weaponObj = Instantiate(weaponPrefabs[0], weaponPos.transform);
            weaponObj.transform.localPosition = Vector3.zero;
            weaponObj.transform.localRotation = Quaternion.identity;
            SetWeapon(weaponObj);
        }   

        if (Input.GetKeyDown(KeyCode.Home))
        {
            Weapon weaponObj = Instantiate(weaponPrefabs[1], weaponPos.transform);
            weaponObj.transform.localPosition = Vector3.zero;
            weaponObj.transform.localRotation = Quaternion.identity;
            SetWeapon(weaponObj);
        }   

        if (Input.GetKeyDown(KeyCode.End))
        {
            Weapon weaponObj = Instantiate(weaponPrefabs[2], weaponPos.transform);
            weaponObj.transform.localPosition = Vector3.zero;
            weaponObj.transform.localRotation = Quaternion.identity;
            SetWeapon(weaponObj);
        }   

        if (Input.GetKeyDown(KeyCode.PageDown))
        {
            Weapon weaponObj = Instantiate(weaponPrefabs[3], weaponPos.transform);
            weaponObj.transform.localPosition = Vector3.zero;
            weaponObj.transform.localRotation = Quaternion.identity;
            SetWeapon(weaponObj);
        }   
    }

    public void SetWeapon(Weapon _weapon)
    {   
        currentWeapon = _weapon;
        currentWeaponType = _weapon.weaponType;
    }
    

    public void Attack()
    {
        if (currentWeapon == null) return;
        currentWeapon.Attack();
    }

    public void Aim()
    {
        if (currentWeapon == null) return;

        if (currentWeaponType != WeaponType.Range) return;

        if (currentWeapon is not Bow bow)
        {
            Debug.Log("활 다운 캐스팅 실패!");
            return;
        }

        bow.Aim();    
    }

    public void AimCancel()
    {
        if (currentWeapon == null) return;

        if (currentWeaponType != WeaponType.Range) return;

        if (currentWeapon is not Bow bow)
        {
            Debug.Log("활 다운 캐스팅 실패!");
            return;
        }

        bow.CancelAim();    
    }

    public void Move(Vector3 move)
    {
        if (currentWeapon == null) return;

        bool bMove = move.sqrMagnitude > 0.001f;
        currentWeapon.Move(bMove);
    }

    public void Sprint(bool flag)
    {
        if (currentWeapon == null) return;

        currentWeapon.Sprint(flag);
    }


    public void Reload()
    {
        if (currentWeapon == null) return;
        
        if (currentWeapon is not RangeWeapon rangeWeapon)
        {
            Debug.Log("Reload() : 원거리 무기 캐스팅 실패!");
            return;
        }

        rangeWeapon.Reload();
    }
}
