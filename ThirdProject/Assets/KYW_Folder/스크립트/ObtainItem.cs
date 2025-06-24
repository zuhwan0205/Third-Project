using UnityEngine;

// 아이템을 획득할 수 있는 컴포넌트
public class ObtainItem : MonoBehaviour, IInteractable
{
    // 인스펙터에서 입력받는 아이템 이름
    [SerializeField] private string itemName;

    // 상호작용 시 InventoryManager를 통해 아이템 획득
    public void Interact()
    {
        Debug.Log($"아이템 : {itemName}");
        if (!string.IsNullOrEmpty(itemName))
        {
            InventoryManager.Instance?.InsertNewItem(itemName);
        }
        // 상호작용 후 오브젝트 파괴
        Destroy(gameObject);
    }

    // UI에 표시할 상호작용 문구 반환
    public string GetInteractText()
    {
        return $"[{itemName}]";
    }
} 