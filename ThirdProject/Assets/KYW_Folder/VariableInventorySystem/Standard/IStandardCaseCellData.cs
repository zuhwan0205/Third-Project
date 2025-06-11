namespace VariableInventorySystem
{
    /// <summary>
    /// 케이스 셀 데이터를 정의하는 인터페이스
    /// </summary>
    public interface IStandardCaseCellData : IVariableInventoryCellData
    {
        /// <summary>
        /// 케이스의 데이터를 가져옵니다
        /// </summary>
        StandardCaseViewData CaseData { get; }
    }
}
