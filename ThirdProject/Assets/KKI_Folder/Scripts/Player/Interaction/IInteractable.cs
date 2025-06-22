public interface IInteractable
{
    void Interact(PlayerInteraction interactor); // 상호작용 요청
    string GetInteractText(); // UI에 표시할 문구 (ex. 빨간 버튼 클릭하기)
}