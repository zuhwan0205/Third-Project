using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace VariableInventorySystem
{
    /// <summary>
    /// 인벤토리 시스템의 핵심 기능을 구현하는 추상 클래스
    /// </summary>
    public abstract class VariableInventoryCore : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        /// <summary>
        /// 인벤토리 뷰 리스트
        /// </summary>
        protected List<IVariableInventoryView> InventoryViews { get; set; } = new List<IVariableInventoryView>();

        /// <summary>
        /// 셀 프리팹
        /// </summary>
        protected virtual GameObject CellPrefab { get; set; }

        /// <summary>
        /// 이펙트 셀의 부모 Transform
        /// </summary>
        protected virtual RectTransform EffectCellParent { get; set; }

        /// <summary>
        /// 현재 선택된 셀
        /// </summary>
        protected IVariableInventoryCell stareCell;

        /// <summary>
        /// 드래그 중인 이펙트 셀
        /// </summary>
        protected IVariableInventoryCell effectCell;

        /// <summary>
        /// 원래 이펙트 셀의 회전 상태
        /// </summary>
        bool? originEffectCellRotate;

        /// <summary>
        /// 현재 커서 위치
        /// </summary>
        Vector2 cursorPosition;

        /// <summary>
        /// 인벤토리 시스템을 초기화합니다
        /// </summary>
        public virtual void Initialize()
        {
            // 이펙트 셀을 생성하고 비활성화 및 선택 불가 상태로 초기화합니다.
            effectCell = Instantiate(CellPrefab, EffectCellParent).GetComponent<IVariableInventoryCell>();
            effectCell.RectTransform.gameObject.SetActive(false);
            effectCell.SetSelectable(false);
        }

        /// <summary>
        /// 인벤토리 뷰를 리스트에 추가하고 셀 콜백을 등록합니다
        /// </summary>
        /// <param name="variableInventoryView">추가할 인벤토리 뷰</param>
        public virtual void AddInventoryView(IVariableInventoryView variableInventoryView)
        {
            InventoryViews.Add(variableInventoryView);
            variableInventoryView.SetCellCallback(OnCellClick, OnCellOptionClick, OnCellEnter, OnCellExit);
        }

        /// <summary>
        /// 인벤토리 뷰를 리스트에서 제거합니다
        /// </summary>
        /// <param name="variableInventoryView">제거할 인벤토리 뷰</param>
        public virtual void RemoveInventoryView(IVariableInventoryView variableInventoryView)
        {
            InventoryViews.Remove(variableInventoryView);
        }

        /// <summary>
        /// 드래그가 시작될 때 호출됩니다. 드래그 준비 및 이펙트 셀 활성화 처리
        /// </summary>
        /// <param name="eventData">드래그 이벤트 데이터</param>
        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }

            foreach (var inventoryViews in InventoryViews)
            {
                inventoryViews.OnPrePick(stareCell);
            }

            var stareData = stareCell?.CellData;
            var isHold = InventoryViews.Any(x => x.OnPick(stareCell));

            if (!isHold)
            {
                return;
            }

            effectCell.RectTransform.gameObject.SetActive(true);
            effectCell.Apply(stareData);
        }

        /// <summary>
        /// 드래그 중일 때 호출됩니다. 이펙트 셀의 위치 업데이트 및 드래그 처리
        /// </summary>
        /// <param name="eventData">드래그 이벤트 데이터</param>
        public virtual void OnDrag(PointerEventData eventData)
        {
            if (effectCell?.CellData == null)
            {
                return;
            }

            foreach (var inventoryViews in InventoryViews)
            {
                inventoryViews.OnDrag(stareCell, effectCell, eventData);
            }

            RectTransformUtility.ScreenPointToLocalPointInRectangle(EffectCellParent, eventData.position, eventData.enterEventCamera, out cursorPosition);

            var (width, height) = GetRotateSize(effectCell.CellData);
            effectCell.RectTransform.localPosition = cursorPosition + new Vector2(
                 -(width - 1) * effectCell.DefaultCellSize.x * 0.5f,
                (height - 1) * effectCell.DefaultCellSize.y * 0.5f);
        }

        /// <summary>
        /// 회전된 셀의 크기를 계산합니다
        /// </summary>
        /// <param name="cell">셀 데이터</param>
        /// <returns>회전된 셀의 (너비, 높이)</returns>
        (int, int) GetRotateSize(IVariableInventoryCellData cell)
        {
            return (cell.IsRotate ? cell.Height : cell.Width, cell.IsRotate ? cell.Width : cell.Height);
        }

        /// <summary>
        /// 드래그가 끝날 때 호출됩니다. 아이템 드롭 처리 및 이펙트 셀 비활성화
        /// </summary>
        /// <param name="eventData">드래그 이벤트 데이터</param>
        public virtual void OnEndDrag(PointerEventData eventData)
        {
            if (effectCell.CellData == null)
            {
                return;
            }

            var isRelease = InventoryViews.Any(x => x.OnDrop(stareCell, effectCell));

            if (!isRelease && originEffectCellRotate.HasValue)
            {
                effectCell.CellData.IsRotate = originEffectCellRotate.Value;
                effectCell.Apply(effectCell.CellData);
                originEffectCellRotate = null;
            }

            foreach (var inventoryViews in InventoryViews)
            {
                inventoryViews.OnDroped(isRelease);
            }

            effectCell.RectTransform.gameObject.SetActive(false);
            effectCell.Apply(null);
        }

        /// <summary>
        /// 아이템의 회전 상태를 전환합니다
        /// </summary>
        public virtual void SwitchRotate()
        {
            if (effectCell.CellData == null)
            {
                return;
            }

            if (!originEffectCellRotate.HasValue)
            {
                originEffectCellRotate = effectCell.CellData.IsRotate;
            }

            effectCell.CellData.IsRotate = !effectCell.CellData.IsRotate;
            effectCell.Apply(effectCell.CellData);

            var (width, height) = GetRotateSize(effectCell.CellData);
            effectCell.RectTransform.localPosition = cursorPosition + new Vector2(
                 -(width - 1) * effectCell.DefaultCellSize.x * 0.5f,
                (height - 1) * effectCell.DefaultCellSize.y * 0.5f);

            foreach (var inventoryViews in InventoryViews)
            {
                inventoryViews.OnSwitchRotate(stareCell, effectCell);
            }
        }

        /// <summary>
        /// 셀 클릭 시 호출되는 메서드
        /// </summary>
        /// <param name="cell">클릭된 셀</param>
        protected virtual void OnCellClick(IVariableInventoryCell cell)
        {
        }

        /// <summary>
        /// 셀 옵션 클릭 시 호출되는 메서드
        /// </summary>
        /// <param name="cell">클릭된 셀</param>
        protected virtual void OnCellOptionClick(IVariableInventoryCell cell)
        {
        }

        /// <summary>
        /// 셀에 마우스가 들어왔을 때 호출되는 메서드
        /// </summary>
        /// <param name="cell">대상 셀</param>
        protected virtual void OnCellEnter(IVariableInventoryCell cell)
        {
            stareCell = cell;

            foreach (var inventoryView in InventoryViews)
            {
                inventoryView.OnCellEnter(stareCell, effectCell);
            }
        }

        /// <summary>
        /// 셀에서 마우스가 나갔을 때 호출되는 메서드
        /// </summary>
        /// <param name="cell">대상 셀</param>
        protected virtual void OnCellExit(IVariableInventoryCell cell)
        {
            foreach (var inventoryView in InventoryViews)
            {
                inventoryView.OnCellExit(stareCell);
            }

            stareCell = null;
        }
    }
}
