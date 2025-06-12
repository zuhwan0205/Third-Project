using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VariableInventorySystem.Sample;

namespace VariableInventorySystem
{
    /// <summary>
    /// 인벤토리 뷰 데이터의 표준 구현체
    /// </summary>
    public class StandardStashViewData : IVariableInventoryViewData
    {
        /// <summary>
        /// 아이템 ID 가져오기
        /// </summary>
        public bool IsDirty { get; set; }
        public IVariableInventoryCellData[] CellData { get; }

        public int CapacityWidth { get; }
        public int CapacityHeight { get; }

        bool[] mask;

        public StandardStashViewData(int capacityWidth, int capacityHeight)
            : this(new IVariableInventoryCellData[capacityWidth * capacityHeight], capacityWidth, capacityHeight)
        {
        }

        public StandardStashViewData(IVariableInventoryCellData[] cellData, int capacityWidth, int capacityHeight)
        {
            Debug.Assert(cellData.Length == capacityWidth * capacityHeight);

            IsDirty = true;
            CellData = cellData;
            CapacityWidth = capacityWidth;
            CapacityHeight = capacityHeight;

            UpdateMask();
        }

        /// <summary>
        /// 아이템 ID 가져오기
        /// </summary>
        public virtual int? GetId(IVariableInventoryCellData cellData)
        {
            for (var i = 0; i < CellData.Length; i++)
            {
                if (CellData[i] == cellData)
                {
                    return i;
                }
            }

            return null;
        }

        /// <summary>
        /// 삽입 가능한 위치 ID 가져오기
        /// </summary>
        public virtual int? GetInsertableId(IVariableInventoryCellData cellData)
        {
            for (var i = 0; i < mask.Length; i++)
            {
                if (!mask[i] && CheckInsert(i, cellData))
                {
                    return i;
                }
            }

            return null;
        }

        /// <summary>
        /// 인벤토리에 아이템 삽입
        /// </summary>
        public virtual void InsertInventoryItem(int id, IVariableInventoryCellData cellData)
        {
            CellData[id] = cellData;
            IsDirty = true;

            UpdateMask();
        }

        /// <summary>
        /// 특정 위치에 아이템 삽입 가능 여부 확인
        /// </summary>
        public virtual bool CheckInsert(int id, IVariableInventoryCellData cellData)
        {
            if (id < 0)
            {
                return false;
            }

            var (width, height) = GetRotateSize(cellData);

            // check width
            if ((id % CapacityWidth) + (width - 1) >= CapacityWidth)
            {
                return false;
            }

            // check height
            if (id + ((height - 1) * CapacityWidth) >= CellData.Length)
            {
                return false;
            }

            for (var i = 0; i < width; i++)
            {
                for (var t = 0; t < height; t++)
                {
                    if (mask[id + i + (t * CapacityWidth)])
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// 마스크 업데이트
        /// </summary>
        protected void UpdateMask()
        {
            mask = new bool[CapacityWidth * CapacityHeight];

            for (var i = 0; i < CellData.Length; i++)
            {
                if (CellData[i] == null || mask[i])
                {
                    continue;
                }

                var width = CellData[i].IsRotate ? CellData[i].Height : CellData[i].Width;
                var height = CellData[i].IsRotate ? CellData[i].Width : CellData[i].Height;

                for (var w = 0; w < width; w++)
                {
                    for (var h = 0; h < height; h++)
                    {
                        var checkIndex = i + w + (h * CapacityWidth);
                        if (checkIndex < mask.Length)
                        {
                            mask[checkIndex] = true;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 회전된 크기 계산
        /// </summary>
        protected (int, int) GetRotateSize(IVariableInventoryCellData cell)
        {
            if (cell == null)
            {
                return (1, 1);
            }

            return (cell.IsRotate ? cell.Height : cell.Width, cell.IsRotate ? cell.Width : cell.Height);
        }

        // 특정 ID의 아이템 삭제
        public virtual void RemoveItemById(int itemId)
        {
            if (itemId < 0 || itemId >= CellData.Length)
                return;

            CellData[itemId] = null;
            IsDirty = true;
            UpdateMask();
        }

        // 특정 타입의 아이템을 찾아 삭제
        public virtual bool RemoveItemByType(int itemType)
        {
            // 모든 셀을 순회하면서
            for (int i = 0; i < CellData.Length; i++)
            {
                // 해당 셀에 아이템이 있고, 타입이 일치하면
                if (CellData[i] != null)
                {
                    var item = CellData[i] as ItemCellData;
                    if (item != null && item.Id == itemType)
                    {
                        CellData[i] = null;
                        IsDirty = true;
                        UpdateMask();
                        return true;  // 첫 번째로 찾은 아이템만 삭제
                    }
                }
            }
            return false;  // 삭제 실패 (해당 타입의 아이템을 찾지 못함)
        }

        // 특정 타입의 아이템 개수 세기
        public virtual int CountItemsByType(int itemType)
        {
            int count = 0;
            for (int i = 0; i < CellData.Length; i++)
            {
                if (CellData[i] != null)
                {
                    var item = CellData[i] as ItemCellData;
                    if (item != null && item.Id == itemType)
                    {
                        count++;
                    }
                }
            }
            return count;
        }
    }
}
