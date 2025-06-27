using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VariableInventorySystem;
using VariableInventorySystem.Sample;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [SerializeField] StandardCore standardCore;
    [SerializeField] StandardStashView standardStashView;
    // [SerializeField] UnityEngine.UI.Button rotateButton;

    private StandardStashViewData stashData;
    private bool isInventoryVisible = true;  // 인벤토리 표시 상태
    
    // 아이템별 개수를 저장하는 Dictionary
    private Dictionary<string, int> itemCounts = new Dictionary<string, int>();

    // 아이템 타입별 이름 정의
    private readonly string[] itemNames = new string[]
    {
        "권총",
        "샷건",
        "칼",
        "도끼",
        "활",
        "총알",
        "화살",
        "포션",
        "통조림"
    };

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // 씬이 변경되어도 파괴되지 않도록 설정

        // 아이템 개수 딕셔너리 초기화
        InitializeItemCounts();

        standardCore.Initialize();
        standardCore.AddInventoryView(standardStashView);

        // rotateButton.onClick.AddListener(standardCore.SwitchRotate);

        StartCoroutine(InsertCoroutine());
    }

    // 아이템 개수 딕셔너리 초기화
    private void InitializeItemCounts()
    {
        foreach (string itemName in itemNames)
        {
            itemCounts[itemName] = 0;
        }
    }

    IEnumerator InsertCoroutine()
    {
        stashData = new StandardStashViewData(12, 12);

        // InsertNewItem("포션");
        // InsertNewItem("통조림");
        ToggleInventory();
        yield return null;
    }
    void Update()
    {
        // I 키를 눌러 인벤토리 토글
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }

        // === 디버깅용 키 입력 ===
        
        // 1키: 포션 추가
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            InsertNewItem("포션");
        }

        // 2키: 통조림 추가  
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            InsertNewItem("통조림");
        }

        // 3키: 권총 추가
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            InsertNewItem("권총");
        }

        // Q키: 포션 삭제
        if (Input.GetKeyDown(KeyCode.Q))
        {
            RemoveItem("포션");
        }

        // W키: 통조림 삭제
        if (Input.GetKeyDown(KeyCode.W))
        {
            RemoveItem("통조림");
        }

        // E키: 권총 삭제
        if (Input.GetKeyDown(KeyCode.E))
        {
            RemoveItem("권총");
        }

        // P키: 모든 아이템 개수 출력
        if (Input.GetKeyDown(KeyCode.P))
        {
            PrintAllItemCounts();
        }

        // C키: 특정 아이템 개수 확인 (포션 예시)
        if (Input.GetKeyDown(KeyCode.C))
        {
            int count = CheckItemCount("포션");
            Debug.Log($"포션 개수: {count}개");
        }
    }

    // 인벤토리 토글 함수
    public void ToggleInventory()
    {
        isInventoryVisible = !isInventoryVisible;
        var canvasGroup = standardStashView.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.alpha = isInventoryVisible ? 1f : 0f;
            canvasGroup.interactable = isInventoryVisible;
            canvasGroup.blocksRaycasts = isInventoryVisible;
        }
    }

    // 아이템 삭제 함수
    public void RemoveItem(string itemName)
    {
        int itemType = System.Array.IndexOf(itemNames, itemName);
        if (itemType == -1) // 아이템 이름을 찾지 못했을 경우
        {
            Debug.LogWarning($"'{itemName}' 아이템을 찾을 수 없습니다.");
            return;
        }

        if (stashData != null)
        {
            if (stashData.RemoveItemByType(itemType))
            {
                // 아이템 개수 감소
                if (itemCounts.ContainsKey(itemName) && itemCounts[itemName] > 0)
                {
                    itemCounts[itemName]--;
                    Debug.Log($"{itemName} 삭제됨. 현재 개수: {itemCounts[itemName]}개");
                    
                    // 탄약이 변경된 경우 UI 업데이트
                    if ((itemName == "총알" || itemName == "화살") && WeaponUIManager.Instance != null)
                    {
                        WeaponUIManager.Instance.UpdateAmmoCount();
                    }
                }
                
                standardStashView.Apply(stashData);
            }
        }
    }

    // 새로운 아이템 삽입 함수
    public void InsertNewItem(string itemName)
    {
        int itemType = System.Array.IndexOf(itemNames, itemName);
        if (itemType == -1) // 아이템 이름을 찾지 못했을 경우
        {
            Debug.LogWarning($"'{itemName}' 아이템을 찾을 수 없습니다.");
            return;
        }

        if (stashData != null)
        {
            var item = new ItemCellData(itemType);
            var insertableId = stashData.GetInsertableId(item);
            
            if (insertableId.HasValue)
            {
                stashData.InsertInventoryItem(insertableId.Value, item);
                
                // 아이템 개수 증가
                if (itemCounts.ContainsKey(itemName))
                {
                    itemCounts[itemName]++;
                    Debug.Log($"{itemName} 추가됨. 현재 개수: {itemCounts[itemName]}개");
                    
                    // 탄약이 변경된 경우 UI 업데이트
                    if ((itemName == "총알" || itemName == "화살") && WeaponUIManager.Instance != null)
                    {
                        WeaponUIManager.Instance.UpdateAmmoCount();
                    }
                }
                
                standardStashView.Apply(stashData);
            }
            else
            {
                Debug.Log("인벤토리가 가득 찼습니다.");
            }
        }
    }

    // 아이템 개수 확인 함수
    public int CheckItemCount(string itemName)
    {
        if (itemCounts.ContainsKey(itemName))
        {
            return itemCounts[itemName];
        }
        else
        {
            Debug.LogWarning($"'{itemName}' 아이템을 찾을 수 없습니다.");
            return 0;
        }
    }

    // 모든 아이템 개수 출력 (디버깅용)
    public void PrintAllItemCounts()
    {
        Debug.Log("=== 현재 아이템 개수 ===");
        foreach (var kvp in itemCounts)
        {
            Debug.Log($"{kvp.Key}: {kvp.Value}개");
        }
    }
}
