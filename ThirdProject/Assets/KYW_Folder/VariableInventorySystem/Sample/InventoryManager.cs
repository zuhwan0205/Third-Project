using System.Collections;
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
    private int itemIndex = 0;  // 현재 아이템 인덱스
    private bool isInventoryVisible = true;  // 인벤토리 표시 상태

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

        standardCore.Initialize();
        standardCore.AddInventoryView(standardStashView);

        // rotateButton.onClick.AddListener(standardCore.SwitchRotate);

        StartCoroutine(InsertCoroutine());
    }

    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.R))
        // {
        //     standardCore.SwitchRotate();
        // }

        // I 키를 눌러 인벤토리 토글
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }

        // 숫자키 1-9를 눌러 해당 타입의 아이템 삭제
        for (int i = 0; i < 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                RemoveItem(itemNames[i]);
            }
        }

        // ASDFG 키를 눌러 아이템 개수 확인
        if (Input.GetKeyDown(KeyCode.A)) CheckItemCount(itemNames[0]);
        else if (Input.GetKeyDown(KeyCode.S)) CheckItemCount(itemNames[1]);
        else if (Input.GetKeyDown(KeyCode.D)) CheckItemCount(itemNames[2]);
        else if (Input.GetKeyDown(KeyCode.F)) CheckItemCount(itemNames[3]);
        else if (Input.GetKeyDown(KeyCode.G)) CheckItemCount(itemNames[4]);
        else if (Input.GetKeyDown(KeyCode.H)) CheckItemCount(itemNames[5]);
        else if (Input.GetKeyDown(KeyCode.J)) CheckItemCount(itemNames[6]);
        else if (Input.GetKeyDown(KeyCode.K)) CheckItemCount(itemNames[7]);
        else if (Input.GetKeyDown(KeyCode.L)) CheckItemCount(itemNames[8]);

        // QWERT 키를 눌러 아이템 삽입
        if (Input.GetKeyDown(KeyCode.Q)) InsertNewItem(itemNames[0]);
        else if (Input.GetKeyDown(KeyCode.W)) InsertNewItem(itemNames[1]);
        else if (Input.GetKeyDown(KeyCode.E)) InsertNewItem(itemNames[2]);
        else if (Input.GetKeyDown(KeyCode.R)) InsertNewItem(itemNames[3]);
        else if (Input.GetKeyDown(KeyCode.T)) InsertNewItem(itemNames[4]);
        else if (Input.GetKeyDown(KeyCode.Y)) InsertNewItem(itemNames[5]);
        else if (Input.GetKeyDown(KeyCode.U)) InsertNewItem(itemNames[6]);
        else if (Input.GetKeyDown(KeyCode.P)) InsertNewItem(itemNames[7]);
        else if (Input.GetKeyDown(KeyCode.O)) InsertNewItem(itemNames[8]);
    }

    // 인벤토리 토글 함수
    public void ToggleInventory()
    {
        isInventoryVisible = !isInventoryVisible;
        standardStashView.gameObject.SetActive(isInventoryVisible);
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
                standardStashView.Apply(stashData);
            }
        }
    }

    // 아이템 개수 확인 함수
    public int CheckItemCount(string itemName)
    {
        int itemType = System.Array.IndexOf(itemNames, itemName);
        if (itemType == -1) // 아이템 이름을 찾지 못했을 경우
        {
            Debug.LogWarning($"'{itemName}' 아이템을 찾을 수 없습니다.");
            return -1; // 아이템을 찾지 못하면 -1 반환
        }

        if (stashData != null)
        {
            int count = stashData.CountItemsByType(itemType);
            Debug.Log($"{itemNames[itemType]} 개수: {count}개"); // 여전히 콘솔에 출력
            return count;
        }
        return 0; // stashData가 null인 경우 0 반환 (또는 다른 적절한 기본값)
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
                standardStashView.Apply(stashData);
            }
            else
            {
                Debug.Log("인벤토리가 가득 찼습니다.");
            }
        }
    }

    IEnumerator InsertCoroutine()
    {
        stashData = new StandardStashViewData(12, 12);

        // var caseItem = new CaseCellData(0);
        // stashData.InsertInventoryItem(stashData.GetInsertableId(caseItem).Value, caseItem);
        // standardStashView.Apply(stashData);
    
        // for (var i = 0; i < 20; i++)
        // {
        //     InsertNewItem(itemNames[i % 9]);
            
        // }
        yield return null;
    }
}
