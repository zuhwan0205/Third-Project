using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace VariableInventorySystem
{
    /// <summary>
    /// 케이스 뷰를 구현하는 클래스
    /// </summary>
    public class StandardCaseView : StandardStashView
    {
        /// <summary>
        /// 아이템이 드롭되었을 때 호출되는 메서드
        /// </summary>
        /// <param name="isDroped">드롭 성공 여부</param>
        public override void OnDroped(bool isDroped)
        {
            base.OnDroped(isDroped);
            ReApply();
        }
    }
}