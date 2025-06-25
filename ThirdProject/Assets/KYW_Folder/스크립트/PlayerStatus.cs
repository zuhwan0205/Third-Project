using System.Collections.Generic;
using UnityEngine;

// 플레이어의 상태(체력, 허기, 닉네임, 아이템 개수)를 관리하는 클래스
public class PlayerStatus : MonoBehaviour
{
    [Header("플레이어 정보")]
    public string NickName;

    [Header("스탯")]
    public float MaxHealth = 100f;
    public float CurrentHealth = 100f;
    public float MaxHunger = 100f;
    public float CurrentHunger = 100f;

    [Header("아이템 개수 관리")]
    // 아이템 이름별 개수 관리
    private Dictionary<string, int> itemCounts = new();

    // 체력 증감
    public void AddHealth(float amount)
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0, MaxHealth);
    }

    // 허기 증감
    public void AddHunger(float amount)
    {
        CurrentHunger = Mathf.Clamp(CurrentHunger + amount, 0, MaxHunger);
    }

    // 아이템 개수 추가
    public void AddItem(string itemName, int count = 1)
    {
        if (!itemCounts.ContainsKey(itemName))
            itemCounts[itemName] = 0;
        itemCounts[itemName] += count;
    }

    // 아이템 개수 감소
    public void RemoveItem(string itemName, int count = 1)
    {
        if (itemCounts.ContainsKey(itemName))
        {
            itemCounts[itemName] -= count;
            if (itemCounts[itemName] <= 0)
                itemCounts.Remove(itemName);
        }
    }

    // 아이템 개수 조회
    public int GetItemCount(string itemName)
    {
        return itemCounts.TryGetValue(itemName, out int count) ? count : 0;
    }
} 