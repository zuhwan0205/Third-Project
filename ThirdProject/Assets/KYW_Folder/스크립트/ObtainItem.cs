using UnityEngine;
using System.Collections;

// 아이템을 획득할 수 있는 컴포넌트
public class InteractableItem : MonoBehaviour, IInteractable
{
    // 인스펙터에서 입력받는 아이템 이름
    [SerializeField] private string itemName;

    private bool isInteracting = false; // 중복 방지

    // 상호작용 시 InventoryManager를 통해 아이템 획득
    public void Interact()
    {
        // 무기일 경우에만 WeaponController의 ownedWeapons 배열 값 true로 변경
        if ((itemName == "도끼" || itemName == "칼" || itemName == "권총" 
            || itemName == "샷건" || itemName == "활"))
        {
            InventoryManager.Instance.InsertNewItem(itemName);
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
        if ((itemName == "포션" || itemName == "통조림" || itemName == "총알" || itemName == "화살" || itemName == "샷건총알" || itemName == "화살") && !isInteracting)
        {
            InventoryManager.Instance.InsertNewItem(itemName);
            Destroy(gameObject);
            return;
        }
        if ((itemName == "상자") && !isInteracting)
        {
            transform.GetComponent<Animation>().Play("ChestOpen");
            Destroy(gameObject, 5f);
            return;
        }



        
        // 큰침대, 작은침대 상호작용: 상호작용바 채우고 체력 회복
        if ((itemName == "큰침대" || itemName == "작은침대") && !isInteracting)
        {
            //인터렉션 코루틴 호출
            StartCoroutine(InteractionCoroutine(2f));

            // 상호작용 완료 후 체력 회복
            PlayerController.Instance.Heal(25);
            
            return;
        }

        if ((itemName == "냉장고") && !isInteracting)
        {
            //인터렉션 코루틴 호출
            StartCoroutine(InteractionCoroutine(2f));

            // 상호작용 완료 후 허기 회복
            PlayerController.Instance.IncreaseHunger(25);
            return;
        }

        
    }
    //여기에 인터렉션 코루틴 추가
    IEnumerator InteractionCoroutine(float second)
    {
        isInteracting = true;
        
        //상호작용바가 2초동안 차오르는 함수 호출
        InteractionBar.Instance.StartInteractionBar(second);
        
        //위함수가 끝날때까지 대기
        yield return new WaitWhile(() => InteractionBar.Instance.IsInteracting());
        
        isInteracting = false;
    }

    // UI에 표시할 상호작용 문구 반환
    public string GetInteractText()
    {
        return $"[{itemName}]와 상호작용";
    }
} 