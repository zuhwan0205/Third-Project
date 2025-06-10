using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace VariableInventorySystem
{
    /// <summary>
    /// 케이스 뷰의 데이터를 관리하는 클래스
    /// </summary>
    public class StandardCaseViewData : StandardStashViewData
    {
        /// <summary>
        /// 케이스 뷰 데이터 생성자
        /// </summary>
        /// <param name="capacityWidth">가로 용량</param>
        /// <param name="capacityHeight">세로 용량</param>
        public StandardCaseViewData(int capacityWidth, int capacityHeight) : base(capacityWidth, capacityHeight)
        {
        }

        /// <summary>
        /// 기존 데이터로 케이스 뷰 데이터 생성자
        /// </summary>
        /// <param name="cellData">셀 데이터 배열</param>
        /// <param name="capacityWidth">가로 용량</param>
        /// <param name="capacityHeight">세로 용량</param>
        public StandardCaseViewData(IVariableInventoryCellData[] cellData, int capacityWidth, int capacityHeight) : base(cellData, capacityWidth, capacityHeight)
        {
        }

        /// <summary>
        /// 아이템을 삽입할 수 있는 위치 ID를 반환
        /// </summary>
        /// <param name="cellData">삽입할 셀 데이터</param>
        /// <returns>삽입 가능한 위치 ID</returns>
        public override int? GetInsertableId(IVariableInventoryCellData cellData)
        {
            if (cellData is IStandardCaseCellData caseData)
            {
                if (caseData.CaseData == this)
                {
                    return null;
                }
            }

            return base.GetInsertableId(cellData);
        }

        /// <summary>
        /// 특정 위치에 아이템 삽입 가능 여부 확인
        /// </summary>
        /// <param name="id">삽입할 위치 ID</param>
        /// <param name="cellData">삽입할 셀 데이터</param>
        /// <returns>삽입 가능 여부</returns>
        public override bool CheckInsert(int id, VariableInventorySystem.IVariableInventoryCellData cellData)
        {
            if (cellData is IStandardCaseCellData caseData)
            {
                if (caseData.CaseData == this)
                {
                    return false;
                }
            }

            return base.CheckInsert(id, cellData);
        }
    }
}
