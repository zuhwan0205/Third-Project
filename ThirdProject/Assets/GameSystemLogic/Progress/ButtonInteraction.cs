using System.Collections;
using UnityEngine;

public class ButtonInteraction : MonoBehaviour, IInteractable
{
    public enum ButtonType { Yes, No }
    public ButtonType buttonType;
    private bool interactable = true;

    public void Interact()
    {
        Debug.Log($"[{buttonType}] 버튼 누름");

        QuestionManager.Instance.OnPlayerAnswered(buttonType == ButtonType.Yes);
        if (buttonType == ButtonType.Yes & interactable)
        {
            GameScene_Button.instance.PushYesButton();
            interactable = false;
        }
        else if(buttonType == ButtonType.No & interactable)
        {
            GameScene_Button.instance.PushNoButton();
            interactable = false;
        }
        StartCoroutine(WaitButton());
    }

    public string GetInteractText()
    {
        return string.Empty; // 버튼엔 아무 표시도 필요 없음
    }
    
    private IEnumerator WaitButton()
    {
        yield return new WaitForSeconds(3);
        interactable = true;
    }
}