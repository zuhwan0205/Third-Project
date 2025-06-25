using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 캐릭터 관련 UI와 상태를 통합 관리하는 클래스
public class CharacterUIManager : MonoBehaviour
{
    public static CharacterUIManager Instance { get; private set; }

    // 인스펙터에서 할당할 UI
    [SerializeField] private TextMeshProUGUI playerName;
    [SerializeField] private Image hbBar;
    [SerializeField] private Image hunggerBar;
    [SerializeField] private Image weaponImage;
    [SerializeField] private Image bulletImage;
    [SerializeField] private TextMeshProUGUI bulletCount;
    [SerializeField] public Slider interactionBar;
    [SerializeField] public Sprite[] weaponSprites;
    [SerializeField] public Sprite[] bulletSprites;

    // 플레이어 정보 및 스탯 (프로퍼티)
    public string NickName { get; set; }
    public float MaxHealth { get; set; } = 100f;
    public float CurrentHealth { get; set; } = 100f;
    public float MaxHunger { get; set; } = 100f;
    public float CurrentHunger { get; set; } = 100f;

    // 아이템 이름별 개수 관리
    private Dictionary<string, int> itemCounts = new();

    // 무기/총알 상태 데이터 (프로퍼티)
    private int currentWeaponIndex = -1;
    public int CurrentWeaponIndex
    {
        get => currentWeaponIndex;
        set { if (currentWeaponIndex != value) { currentWeaponIndex = value; UpdateWeaponUI(); } }
    }

    private string currentWeaponName = "";
    public string CurrentWeaponName
    {
        get => currentWeaponName;
        set { if (currentWeaponName != value) { currentWeaponName = value; UpdateWeaponUI(); } }
    }

    private int bulletCountValue = 0;
    public int BulletCountValue
    {
        get => bulletCountValue;
        set { if (bulletCountValue != value) { bulletCountValue = value; UpdateWeaponUI(); } }
    }

    private int arrowCountValue = 0;
    public int ArrowCountValue { get => arrowCountValue; set { if (arrowCountValue != value) { arrowCountValue = value; UpdateWeaponUI(); } } }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void SetPlayerName(string name)
    {
        NickName = name;
        if (playerName != null)
            playerName.text = name;
    }

    public void AddHealth(float amount) => CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0, MaxHealth);
    public void AddHunger(float amount) => CurrentHunger = Mathf.Clamp(CurrentHunger + amount, 0, MaxHunger);

    public void AddItem(string itemName, int count = 1)
    {
        if (!itemCounts.ContainsKey(itemName))
            itemCounts[itemName] = 0;
        itemCounts[itemName] += count;
    }
    public void RemoveItem(string itemName, int count = 1)
    {
        if (itemCounts.ContainsKey(itemName))
        {
            itemCounts[itemName] -= count;
            if (itemCounts[itemName] <= 0)
                itemCounts.Remove(itemName);
        }
    }
    public int GetItemCount(string itemName) => itemCounts.TryGetValue(itemName, out int count) ? count : 0;

    public void SetHBbarFill(float amount) { if (hbBar != null) hbBar.fillAmount = amount; }
    public void SetHunggerBarFill(float amount) { if (hunggerBar != null) hunggerBar.fillAmount = amount; }
    public void SetWeaponImage(Sprite sprite) { if (weaponImage != null) weaponImage.sprite = sprite; }
    public void SetBulletImage(Sprite sprite) { if (bulletImage != null) bulletImage.sprite = sprite; }
    public void SetBulletCount(string count) { if (bulletCount != null) bulletCount.text = count; }
    public void SetInteractionBarFill(float amount) { if (interactionBar != null) interactionBar.value = amount; }

    // 무기 스프라이트 인덱스로 변경
    public void SetWeaponSpriteByIndex(int idx)
    {
        if (weaponImage != null && weaponSprites != null && idx >= 0 && idx < weaponSprites.Length)
            weaponImage.sprite = weaponSprites[idx];
    }
    // 총알 스프라이트 인덱스로 변경
    public void SetBulletSpriteByIndex(int idx)
    {
        if (bulletImage != null && bulletSprites != null && idx >= 0 && idx < bulletSprites.Length)
            bulletImage.sprite = bulletSprites[idx];
    }
    // 무기 스프라이트 이름으로 변경
    public void SetWeaponSpriteByName(string name)
    {
        if (weaponImage != null && weaponSprites != null)
        {
            foreach (var weaponSprite in weaponSprites)
            {
                if (weaponSprite != null && weaponSprite.name == name)
                {
                    weaponImage.sprite = weaponSprite;
                    break;
                }
            }
        }
    }
    // 총알 스프라이트 이름으로 변경
    public void SetBulletSpriteByName(string name)
    {
        if (bulletImage != null && bulletSprites != null)
        {
            foreach (var bulletSprite in bulletSprites)
            {
                if (bulletSprite != null && bulletSprite.name == name)
                {
                    bulletImage.sprite = bulletSprite;
                    break;
                }
            }
        }
    }

    // UI 일괄 업데이트 함수
    public void UpdateWeaponUI()
    {
        // 무기 스프라이트
        SetWeaponSpriteByIndex(currentWeaponIndex);
        // 총알/화살 스프라이트 (무기 종류에 따라 다르게)
        if (currentWeaponName == "샷건" || currentWeaponName == "권총")
        {
            SetBulletSpriteByIndex(currentWeaponIndex);
            SetBulletCount(bulletCountValue.ToString());
        }
        else if (currentWeaponName == "활")
        {
            SetBulletSpriteByIndex(currentWeaponIndex); // 화살 이미지도 bulletSprites에 넣는다고 가정
            SetBulletCount(arrowCountValue.ToString());
        }
        else
        {
            SetBulletCount(""); // 근접무기 등은 총알/화살 없음
        }
    }
} 