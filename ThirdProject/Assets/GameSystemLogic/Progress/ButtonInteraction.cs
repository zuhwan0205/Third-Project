using UnityEngine;

public class ButtonInteraction : MonoBehaviour, IInteractable
{
    public enum ButtonType { Yes, No }
    public ButtonType buttonType;

    public void Interact()
    {
        Debug.Log($"[{buttonType}] 버튼 누름");

        QuestionManager.Instance.OnPlayerAnswered(buttonType == ButtonType.Yes);
    }

    public string GetInteractText()
    {
        return string.Empty; // 버튼엔 아무 표시도 필요 없음
    }
}