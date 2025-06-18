using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Event_MainScene : MonoBehaviour
{
    public static event Action OnLobbyButtonClicked;
    public static event Action OnSettingButtonClicked;
    public static event Action OnQuitButtonClicked;
    public static event Action OnTestButtonClicked;
    
    public static event Action OnCloseSettingButtonClicked;
    
    [SerializeField] private Button LobbyButton;
    [SerializeField] private Button SettingButton;
    [SerializeField] private Button QuitButton;
    [SerializeField] private Button TestButton;
    [SerializeField] private Button CloseSettingButton;
     
    
    IEnumerator WaitSecond(Button button)
    {
        yield return new WaitForSeconds(0.5f);
        button.interactable = true;
    }
    
    public void OnClickLobbyGame(){
        LobbyButton.interactable = false;   
        StartCoroutine(WaitSecond(LobbyButton));
        OnLobbyButtonClicked?.Invoke();
    }
    
    public void OnClickSetting(){
        SettingButton.interactable = false;   
        StartCoroutine(WaitSecond(SettingButton));
        OnSettingButtonClicked?.Invoke();
    }
    
    public void OnClickQuit(){
        QuitButton.interactable = false;   
        StartCoroutine(WaitSecond(QuitButton));
        OnQuitButtonClicked?.Invoke();
    }
    
    public void OnTest(){
        LobbyButton.interactable = false;   
        StartCoroutine(WaitSecond(TestButton));
        OnTestButtonClicked?.Invoke();
    }

    public void OnClickCloseSetting()
    {
        CloseSettingButton.interactable = false;
        StartCoroutine(WaitSecond(CloseSettingButton));
        OnCloseSettingButtonClicked?.Invoke();
    }
    
}
