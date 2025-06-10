using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace VariableInventorySystem
{
    /// <summary>
    /// 케이스 뷰 팝업을 관리하는 클래스
    /// </summary>
    public class StandardCaseViewPopup : MonoBehaviour
    {
        [SerializeField] StandardCaseView standardCaseView;
        [SerializeField] RawImage icon;
        [SerializeField] StandardButton closeButton;

        [SerializeField] RectTransform sizeSampleTarget;
        [SerializeField] RectTransform sizeTarget;
        [SerializeField] Vector2 sizeTargetOffset;

        /// <summary>
        /// 케이스 뷰 컴포넌트를 가져옵니다
        /// </summary>
        public StandardCaseView StandardCaseView => standardCaseView;

        /// <summary>
        /// 에셋 로더 컴포넌트
        /// </summary>
        protected virtual StandardAssetLoader Loader { get; set; } = new StandardAssetLoader();

        /// <summary>
        /// 케이스 팝업을 엽니다
        /// </summary>
        /// <param name="caseData">케이스 데이터</param>
        /// <param name="onCloseButton">닫기 버튼 클릭 시 호출될 콜백</param>
        public virtual void Open(IStandardCaseCellData caseData, Action onCloseButton)
        {
            standardCaseView.Apply(caseData.CaseData);
            StartCoroutine(Loader.LoadAsync(caseData.ImageAsset, tex => icon.texture = tex));
            closeButton.SetCallback(() => onCloseButton());

            // wait for relayout
            StartCoroutine(DelayFrame(() => sizeTarget.sizeDelta = sizeSampleTarget.rect.size + sizeTargetOffset));
        }

        /// <summary>
        /// 한 프레임 대기 후 액션을 실행합니다
        /// </summary>
        /// <param name="action">실행할 액션</param>
        IEnumerator DelayFrame(Action action)
        {
            yield return null;
            action?.Invoke();
        }
    }
}
