using VariableInventorySystem;

namespace VariableInventorySystem.Sample
{
    /// <summary>
    /// 일반 아이템의 데이터를 정의하는 클래스
    /// IVariableInventoryCellData 인터페이스를 구현
    /// </summary>
    public class ItemCellData : IVariableInventoryCellData
    {
        // 기본 속성들
        public int Id { get; private set; }  // 아이템 타입 ID
        public int Width { get; private set; }  // 아이템의 가로 크기
        public int Height { get; private set; } // 아이템의 세로 크기
        public bool IsRotate { get; set; }      // 회전 상태
        public IVariableInventoryAsset ImageAsset { get; }  // 아이템 이미지

        /// <summary>
        /// 아이템 데이터 생성자
        /// </summary>
        /// <param name="sampleSeed">아이템 종류를 결정하는 시드 값</param>
        public ItemCellData(int sampleSeed)
        {
            Id = sampleSeed;  // 아이템 타입 ID 설정

            // 시드 값에 따라 다른 무기 아이템 생성
            switch (sampleSeed)
            {
                case 0: // 권총 1
                    Width = 2; Height = 1;
                    ImageAsset = new VariableInventorySystem.StandardAsset("Image/handgun");
                    break;
                case 1: // 권총 2
                    Width = 2; Height = 1;
                    ImageAsset = new VariableInventorySystem.StandardAsset("Image/handgun2");
                    break;
                case 2: // 소총
                    Width = 4; Height = 2;
                    ImageAsset = new VariableInventorySystem.StandardAsset("Image/rifle");
                    break;
                case 3: // 저격총
                    Width = 5; Height = 2;
                    ImageAsset = new VariableInventorySystem.StandardAsset("Image/sniper");
                    break;
                case 4: // 기관단총
                    Width = 2; Height = 2;
                    ImageAsset = new VariableInventorySystem.StandardAsset("Image/submachinegun");
                    break;
                case 5: // 권총 1 (중복)
                    Width = 2; Height = 1;
                    ImageAsset = new VariableInventorySystem.StandardAsset("Image/handgun");
                    break;
                case 6: // 권총 2 (중복)
                    Width = 2; Height = 1;
                    ImageAsset = new VariableInventorySystem.StandardAsset("Image/handgun2");
                    break;
            }
        }
    }
}