using System;
using UnityEngine;

namespace VariableInventorySystem
{
    /// <summary>
    /// 인벤토리 셀의 기본 클래스
    /// </summary>
    public abstract class VariableInventoryCell : MonoBehaviour, IVariableInventoryCell
    {
        /// <summary>
        /// 셀의 RectTransform 컴포넌트
        /// </summary>
        public RectTransform RectTransform => (RectTransform)transform;

        /// <summary>
        /// 셀의 데이터
        /// </summary>
        public IVariableInventoryCellData CellData { get; protected set; }

        /// <summary>
        /// 셀의 기본 크기
        /// </summary>
        public virtual Vector2 DefaultCellSize { get; set; }

        /// <summary>
        /// 셀 사이의 여백
        /// </summary>
        public virtual Vector2 MargineSpace { get; set; }

        protected virtual IVariableInventoryCellActions ButtonActions { get; set; }

        /// <summary>
        /// 셀의 이벤트 처리를 위한 액션 설정
        /// </summary>
        public virtual void SetCellCallback(
            Action<IVariableInventoryCell> onPointerClick,
            Action<IVariableInventoryCell> onPointerOptionClick,
            Action<IVariableInventoryCell> onPointerEnter,
            Action<IVariableInventoryCell> onPointerExit,
            Action<IVariableInventoryCell> onPointerDown,
            Action<IVariableInventoryCell> onPointerUp)
        {
            ButtonActions.SetCallback(
                () => onPointerClick?.Invoke(this),
                () => onPointerOptionClick?.Invoke(this),
                () => onPointerEnter?.Invoke(this),
                () => onPointerExit?.Invoke(this),
                () => onPointerDown?.Invoke(this),
                () => onPointerUp?.Invoke(this));
        }

        /// <summary>
        /// 셀에 새로운 데이터 적용
        /// </summary>
        /// <param name="cellData">적용할 셀 데이터</param>
        public void Apply(IVariableInventoryCellData cellData)
        {
            CellData = cellData;
            OnApply();
        }

        /// <summary>
        /// 셀의 선택 가능 여부 설정
        /// </summary>
        /// <param name="value">선택 가능 여부</param>
        public abstract void SetSelectable(bool value);

        /// <summary>
        /// 데이터 적용 시 호출되는 추상 메서드
        /// </summary>
        protected abstract void OnApply();
    }
}
