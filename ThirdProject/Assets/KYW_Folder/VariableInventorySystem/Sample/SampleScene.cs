using System.Collections;
using UnityEngine;
using VariableInventorySystem;
using VariableInventorySystem.Sample;

public class SampleScene : MonoBehaviour
{
    [SerializeField] StandardCore standardCore;
    [SerializeField] StandardStashView standardStashView;
    [SerializeField] UnityEngine.UI.Button rotateButton;

    private StandardStashViewData stashData;
    private int itemIndex = 0;  // 현재 아이템 인덱스
    private bool isInventoryVisible = true;  // 인벤토리 표시 상태

    // 아이템 타입별 이름 정의
    private readonly string[] itemNames = new string[]
    {
        "권총 1",
        "권총 2",
        "소총",
        "저격총",
        "기관단총"
    };

    void Awake()
    {
        standardCore.Initialize();
        standardCore.AddInventoryView(standardStashView);

        rotateButton.onClick.AddListener(standardCore.SwitchRotate);

        StartCoroutine(InsertCoroutine());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            standardCore.SwitchRotate();
        }

        // I 키를 눌러 인벤토리 토글
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }

        // 숫자키 1-5를 눌러 해당 타입의 아이템 삭제
        for (int i = 0; i < 5; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                RemoveItem(i);
            }
        }

        // ASDFG 키를 눌러 아이템 개수 확인
        if (Input.GetKeyDown(KeyCode.A)) CheckItemCount(0);
        else if (Input.GetKeyDown(KeyCode.S)) CheckItemCount(1);
        else if (Input.GetKeyDown(KeyCode.D)) CheckItemCount(2);
        else if (Input.GetKeyDown(KeyCode.F)) CheckItemCount(3);
        else if (Input.GetKeyDown(KeyCode.G)) CheckItemCount(4);

        // QWERT 키를 눌러 아이템 삽입
        if (Input.GetKeyDown(KeyCode.Q)) InsertNewItem(0);
        else if (Input.GetKeyDown(KeyCode.W)) InsertNewItem(1);
        else if (Input.GetKeyDown(KeyCode.E)) InsertNewItem(2);
        else if (Input.GetKeyDown(KeyCode.R)) InsertNewItem(3);
        else if (Input.GetKeyDown(KeyCode.T)) InsertNewItem(4);
    }

    // 인벤토리 토글 함수
    private void ToggleInventory()
    {
        isInventoryVisible = !isInventoryVisible;
        standardStashView.gameObject.SetActive(isInventoryVisible);
    }

    // 아이템 삭제 함수
    private void RemoveItem(int itemType)
    {
        if (stashData != null)
        {
            if (stashData.RemoveItemByType(itemType))
            {
                standardStashView.Apply(stashData);
            }
        }
    }

    // 아이템 개수 확인 함수
    private void CheckItemCount(int itemType)
    {
        if (stashData != null)
        {
            int count = stashData.CountItemsByType(itemType);
            Debug.Log($"{itemNames[itemType]} 개수: {count}개");
        }
    }

    // 새로운 아이템 삽입 함수
    private void InsertNewItem(int itemType)
    {
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
        stashData = new StandardStashViewData(12, 8);

        // var caseItem = new CaseCellData(0);
        // stashData.InsertInventoryItem(stashData.GetInsertableId(caseItem).Value, caseItem);
        // standardStashView.Apply(stashData);
    
        for (var i = 0; i < 20; i++)
        {
            InsertNewItem(i % 5);
            yield return null;
        }
    }
}
