using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponUIManager : MonoBehaviour
{
    public static WeaponUIManager Instance { get; private set; }

    [Header("무기 UI")]
    [SerializeField] Image weaponImage;
    
    [Header("탄약 UI")]
    [SerializeField] Image ammoImage;
    [SerializeField] TextMeshProUGUI ammoCountText;
    
    // 마지막으로 업데이트된 장탄 수를 저장
    private int currentLoadedAmmo = 0;

    [Header("무기별 스프라이트")]
    [SerializeField] Sprite axeSprite;
    [SerializeField] Sprite swordSprite;
    [SerializeField] Sprite bowSprite;
    [SerializeField] Sprite pistolSprite;
    [SerializeField] Sprite shotgunSprite;
    
    [Header("탄약 스프라이트")]
    [SerializeField] Sprite bulletSprite;
    [SerializeField] Sprite arrowSprite;
    [SerializeField] Sprite shotgunShellSprite;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        // 초기에는 무기 없음 상태로 설정
        UpdateWeaponUI(WeaponType.None);
    }

    // 무기 교체 시 호출되는 기본 UI 업데이트 함수
    // public void UpdateWeaponUI(WeaponType weaponType)
    // {
    //     // 핵심 로직을 담은 오버로딩 함수를 loadedAmmo = 0 으로 호출
    //     Debug.Log("1번째");
    //     UpdateWeaponUI(weaponType, 0);
    // }
    
    // 장탄수를 포함하여 UI를 업데이트하는 핵심 오버로딩 함수
    public void UpdateWeaponUI(WeaponType weaponType, int loadedAmmo = 0)
    {
        
    

        Debug.Log("2번째");
        currentLoadedAmmo = loadedAmmo; // 전달받은 장탄수 저장
        
        // 1. 무기 이미지 업데이트
        UpdateWeaponImage(weaponType);
        
        // 2. 탄약 UI 업데이트
        string totalAmmoString;
        Sprite ammoSprite;

        switch (weaponType)
        {
            case WeaponType.Pistol:
                totalAmmoString = InventoryManager.Instance.CheckItemCount("총알").ToString();
                ammoSprite = bulletSprite;
                break;
            case WeaponType.Shotgun:
                totalAmmoString = InventoryManager.Instance.CheckItemCount("샷건총알").ToString();
                ammoSprite = shotgunShellSprite;
                break;
            case WeaponType.Bow:
                totalAmmoString = InventoryManager.Instance.CheckItemCount("화살").ToString();
                ammoSprite = arrowSprite;
                break;
            default:
                // 탄약이 필요없는 무기 (UI 숨기기)
                ammoImage.sprite = null;
                ammoImage.color = Color.clear;
                ammoCountText.text = "";
                return;
        }

        ammoImage.sprite = ammoSprite;
        ammoImage.color = Color.white;
        ammoCountText.text = $"{currentLoadedAmmo} / {totalAmmoString}";
        ammoCountText.color = Color.white;
    }

    // 보유 탄약 개수가 바뀔 때 호출 (인벤토리 매니저 등에서)
    public void UpdateAmmoCount()
    {
        // 현재 무기 타입과, 마지막으로 저장된 장탄수를 이용해 UI를 다시 그림
        WeaponType currentWeapon = WeaponController.Instance.currentWeaponType;
        UpdateWeaponUI(currentWeapon, currentLoadedAmmo);
    }
    
    // 무기 이미지 업데이트 (헬퍼 함수)
    private void UpdateWeaponImage(WeaponType weaponType)
    {
        Sprite weaponSprite = GetWeaponSprite(weaponType);
        
        if (weaponSprite != null)
        {
            weaponImage.sprite = weaponSprite;
            weaponImage.color = Color.white; // 이미지 보이게
        }
        else
        {
            weaponImage.sprite = null;
            weaponImage.color = Color.clear; // 이미지 숨기기
        }
    }

    // 무기 타입에 따른 스프라이트 반환
    private Sprite GetWeaponSprite(WeaponType weaponType)
    {
        switch (weaponType)
        {
            case WeaponType.Axe:
                return axeSprite;
            case WeaponType.ShortSword:
                return swordSprite;
            case WeaponType.Bow:
                return bowSprite;
            case WeaponType.Pistol:
                return pistolSprite;
            case WeaponType.Shotgun:
                return shotgunSprite;
            default:
                return null;
        }
    }
} 