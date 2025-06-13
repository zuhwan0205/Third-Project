using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponController : MonoBehaviour
{
    public WeaponType weaponType;
    public Weapon weapon;

    public Weapon[] weaponPrefabs;
    public Transform weaponPos;
    
    void Start()
    {
        weaponType = WeaponType.None;   
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
    }

    public void SetWeapon(Weapon _weapon)
    {   
        weapon = _weapon;
        weaponType = _weapon.weaponType;

        switch(weaponType)
        {
            case WeaponType.Axe:
            {
                if (weapon is Axe myAxe)
                {
                    // myAxe만의 특수 초기화/로직
                }
                break;
            }
        }
    }
    

}
