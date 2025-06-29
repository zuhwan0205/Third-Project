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

    // 무기 UI 업데이트
    public void UpdateWeaponUI(WeaponType weaponType)
    {
        // 무기 이미지 업데이트
        UpdateWeaponImage(weaponType);
        
        // 탄약 UI 업데이트
        UpdateAmmoUI(weaponType);
    }

    // 무기 이미지 업데이트
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

    // 탄약 UI 업데이트
    private void UpdateAmmoUI(WeaponType weaponType)
    {
        switch (weaponType)
        {
            case WeaponType.Pistol:
                // 총알 UI 표시
                ammoImage.sprite = bulletSprite;
                ammoImage.color = Color.white;
                int bulletCount = InventoryManager.Instance.CheckItemCount("총알");
                ammoCountText.text = bulletCount.ToString();
                ammoCountText.color = Color.white;
                break;
            
            case WeaponType.Shotgun:
                // 샷건탄 UI 표시
                ammoImage.sprite = shotgunShellSprite;
                ammoImage.color = Color.white;
                int shellCount = InventoryManager.Instance.CheckItemCount("샷건총알");
                ammoCountText.text = shellCount.ToString();
                ammoCountText.color = Color.white;
                break;
                
            case WeaponType.Bow:
                // 화살 UI 표시
                ammoImage.sprite = arrowSprite;
                ammoImage.color = Color.white;
                int arrowCount = InventoryManager.Instance.CheckItemCount("화살");
                ammoCountText.text = arrowCount.ToString();
                ammoCountText.color = Color.white;
                break;
                
            default:
                // 탄약이 필요없는 무기이거나 무기가 없는 경우
                ammoImage.sprite = null;
                ammoImage.color = Color.clear;
                ammoCountText.text = "";
                ammoCountText.color = Color.clear;
                break;
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

    // 탄약 개수만 업데이트 (아이템을 사용했을 때 호출)
    public void UpdateAmmoCount()
    {
        WeaponType currentWeapon = WeaponController.Instance.currentWeaponType;
        
        switch (currentWeapon)
        {
            case WeaponType.Pistol:
                int bulletCount = InventoryManager.Instance.CheckItemCount("총알");
                ammoCountText.text = bulletCount.ToString();
                break;
            
            case WeaponType.Shotgun:
                int shellCount = InventoryManager.Instance.CheckItemCount("샷건총알");
                ammoCountText.text = shellCount.ToString();
                break;
                
            case WeaponType.Bow:
                int arrowCount = InventoryManager.Instance.CheckItemCount("화살");
                ammoCountText.text = arrowCount.ToString();
                break;
        }
    }
} 