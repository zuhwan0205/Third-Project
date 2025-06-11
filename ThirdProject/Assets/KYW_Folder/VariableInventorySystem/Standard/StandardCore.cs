using System.Collections.Generic;
using UnityEngine;

namespace VariableInventorySystem
{
    /// <summary>
    /// 표준 인벤토리 시스템의 핵심 클래스
    /// </summary>
    public class StandardCore : VariableInventoryCore
    {
        [SerializeField] GameObject cellPrefab;
        [SerializeField] GameObject casePopupPrefab;
        [SerializeField] RectTransform effectCellParent;
        [SerializeField] RectTransform caseParent;

        /// <summary>
        /// 셀 프리팹을 가져옵니다
        /// </summary>
        protected override GameObject CellPrefab => cellPrefab;

        /// <summary>
        /// 이펙트 셀의 부모 Transform을 가져옵니다
        /// </summary>
        protected override RectTransform EffectCellParent => effectCellParent;

        /// <summary>
        /// 팝업 리스트
        /// </summary>
        protected List<IStandardCaseCellData> popupList = new List<IStandardCaseCellData>();

        /// <summary>
        /// 셀 클릭 시 호출되는 메서드
        /// </summary>
        /// <param name="cell">클릭된 셀</param>
        protected override void OnCellClick(IVariableInventoryCell cell)
        {
            if (cell.CellData is IStandardCaseCellData caseData)
            {
                if (popupList.Contains(caseData))
                {
                    return;
                }

                popupList.Add(caseData);

                var standardCaseViewPopup = Instantiate(casePopupPrefab, caseParent).GetComponent<StandardCaseViewPopup>();
                AddInventoryView(standardCaseViewPopup.StandardCaseView);

                standardCaseViewPopup.Open(
                    caseData,
                    () =>
                    {
                        RemoveInventoryView(standardCaseViewPopup.StandardCaseView);
                        Destroy(standardCaseViewPopup.gameObject);
                        popupList.Remove(caseData);
                    });
            }
        }
    }
}
