using UnityEngine;
using UnityEngine.UI;

namespace VariableInventorySystem
{
    /// <summary>
    /// 표준 인벤토리 셀을 구현하는 클래스
    /// </summary>
    public class StandardCell : VariableInventoryCell
    {
        [SerializeField] Vector2 defaultCellSize;
        [SerializeField] Vector2 margineSpace;

        [SerializeField] RectTransform sizeRoot;
        [SerializeField] RectTransform target;
        [SerializeField] Graphic background;
        [SerializeField] RawImage cellImage;
        [SerializeField] Graphic highlight;

        [SerializeField] StandardButton button;

        /// <summary>
        /// 기본 셀 크기를 가져옵니다
        /// </summary>
        public override Vector2 DefaultCellSize => defaultCellSize;

        /// <summary>
        /// 여백 공간을 가져옵니다
        /// </summary>
        public override Vector2 MargineSpace => margineSpace;

        /// <summary>
        /// 버튼 액션을 가져옵니다
        /// </summary>
        protected override IVariableInventoryCellActions ButtonActions => button;

        /// <summary>
        /// 에셋 로더 컴포넌트
        /// </summary>
        protected virtual StandardAssetLoader Loader { get; set; }

        protected bool isSelectable = true;
        protected IVariableInventoryAsset currentImageAsset;

        /// <summary>
        /// 셀의 크기를 가져옵니다
        /// </summary>
        /// <returns>셀의 크기</returns>
        public Vector2 GetCellSize()
        {
            var width = ((CellData?.Width ?? 1) * (defaultCellSize.x + margineSpace.x)) - margineSpace.x;
            var height = ((CellData?.Height ?? 1) * (defaultCellSize.y + margineSpace.y)) - margineSpace.y;
            return new Vector2(width, height);
        }

        /// <summary>
        /// 회전된 셀의 크기를 가져옵니다
        /// </summary>
        /// <returns>회전된 셀의 크기</returns>
        public Vector2 GetRotateCellSize()
        {
            var isRotate = CellData?.IsRotate ?? false;
            var cellSize = GetCellSize();
            if (isRotate)
            {
                var tmp = cellSize.x;
                cellSize.x = cellSize.y;
                cellSize.y = tmp;
            }

            return cellSize;
        }

        /// <summary>
        /// 셀의 선택 가능 여부를 설정합니다
        /// </summary>
        /// <param name="value">선택 가능 여부</param>
        public override void SetSelectable(bool value)
        {
            ButtonActions.SetActive(value);
            isSelectable = value;
        }

        /// <summary>
        /// 하이라이트 상태를 설정합니다
        /// </summary>
        /// <param name="value">하이라이트 여부</param>
        public virtual void SetHighLight(bool value)
        {
            highlight.gameObject.SetActive(value);
        }

        /// <summary>
        /// 셀 데이터가 적용될 때 호출됩니다
        /// </summary>
        protected override void OnApply()
        {
            SetHighLight(false);
            target.gameObject.SetActive(CellData != null);
            ApplySize();

            if (CellData == null)
            {
                cellImage.gameObject.SetActive(false);
                background.gameObject.SetActive(false);
            }
            else
            {
                // update cell image
                if (currentImageAsset != CellData.ImageAsset)
                {
                    currentImageAsset = CellData.ImageAsset;

                    cellImage.gameObject.SetActive(false);
                    if (Loader == null)
                    {
                        Loader = new StandardAssetLoader();
                    }

                    StartCoroutine(Loader.LoadAsync(CellData.ImageAsset, tex =>
                    {
                        cellImage.texture = tex;
                        cellImage.gameObject.SetActive(true);
                    }));
                }

                background.gameObject.SetActive(true && isSelectable);
            }
        }

        /// <summary>
        /// 셀의 크기를 적용합니다
        /// </summary>
        protected virtual void ApplySize()
        {
            sizeRoot.sizeDelta = GetRotateCellSize();
            target.sizeDelta = GetCellSize();
            target.localEulerAngles = Vector3.forward * (CellData?.IsRotate ?? false ? 90 : 0);
        }
    }
}
