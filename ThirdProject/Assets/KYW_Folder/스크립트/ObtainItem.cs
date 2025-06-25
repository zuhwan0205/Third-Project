using UnityEngine;
using System.Collections;

// 아이템을 획득할 수 있는 컴포넌트
public class InteractableItem : MonoBehaviour, IInteractable
{
    // 인스펙터에서 입력받는 아이템 이름
    [SerializeField] private string itemName;

    private bool isInteracting = false; // 중복 방지
    private const float interactionTime = 2f; // x초 (예시: 2초)
    private const float healAmount = 30f; // y만큼 체력 회복 (예시: 30)

    // 상호작용 시 InventoryManager를 통해 아이템 획득
    public void Interact()
    {
        if (!string.IsNullOrEmpty(itemName))
        {
            // 무기일 경우에만 WeaponController의 ownedWeapons 배열 값 true로 변경
            if (WeaponController.Instance != null &&
                (itemName == "도끼" || itemName == "칼" || itemName == "권총" 
                || itemName == "샷건" || itemName == "활"))
            {
                InventoryManager.Instance?.InsertNewItem(itemName);
                int idx = -1;
                switch (itemName)
                {
                    case "도끼": idx = 0; break;
                    case "칼": idx = 1; break;
                    case "권총": idx = 2; break;
                    case "샷건": idx = 3; break;
                    case "활": idx = 4; break;
                }
                if (idx >= 0 && idx < WeaponController.Instance.ownedWeapons.Length)
                    WeaponController.Instance.ownedWeapons[idx] = true;

                Destroy(gameObject);
                return;
            }
            // 큰침대, 작은침대 상호작용: 상호작용바 채우고 체력 회복
            if ((itemName == "큰침대" || itemName == "작은침대") && !isInteracting)
            {
                StartCoroutine(InteractionBarAndHealCoroutine());
                return;
            }
        }
    }

    private IEnumerator InteractionBarAndHealCoroutine()
    {
        isInteracting = true;
        // 상호작용바 켜기
        if (CharacterUIManager.Instance != null && CharacterUIManager.Instance.interactionBar != null)
            CharacterUIManager.Instance.interactionBar.gameObject.SetActive(true);
        float timer = 0f;
        while (timer < interactionTime)
        {
            timer += Time.deltaTime;
            float fill = Mathf.Clamp01(timer / interactionTime);
            CharacterUIManager.Instance.SetInteractionBarFill(fill);
            yield return null;
        }
        CharacterUIManager.Instance.SetInteractionBarFill(0f); // 바 초기화
        CharacterUIManager.Instance.AddHealth(healAmount);
        float healthFill = CharacterUIManager.Instance.CurrentHealth / CharacterUIManager.Instance.MaxHealth;
        CharacterUIManager.Instance.SetHBbarFill(healthFill);
        isInteracting = false;
        // 상호작용바 끄기
        if (CharacterUIManager.Instance != null && CharacterUIManager.Instance.interactionBar != null)
            CharacterUIManager.Instance.interactionBar.gameObject.SetActive(false);
        Destroy(gameObject);
    }

    // UI에 표시할 상호작용 문구 반환
    public string GetInteractText()
    {
        return $"[{itemName}]와 상호작용";
    }
} 