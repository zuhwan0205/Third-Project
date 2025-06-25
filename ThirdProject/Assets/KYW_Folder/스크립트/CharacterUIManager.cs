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

    // 플레이어 정보 및 스탯 (프로퍼티)
    public string NickName { get; set; }
    public float MaxHealth { get; set; } = 100f;
    public float CurrentHealth { get; set; } = 100f;
    public float MaxHunger { get; set; } = 100f;
    public float CurrentHunger { get; set; } = 100f;

    // 아이템 이름별 개수 관리
    private Dictionary<string, int> itemCounts = new();

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
} 