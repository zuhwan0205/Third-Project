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
    
    [SerializeField] private Button LobbyButton;
    [SerializeField] private Button SettingButton;
    [SerializeField] private Button QuitButton;
    [SerializeField] private Button TestButton;
    
    IEnumerator WaitSecond(Button button)
    {
        yield return new WaitForSeconds(2);
        button.interactable = true;
    }
    
    public void OnClickLobbyGame(){
        LobbyButton.interactable = false;   
        StartCoroutine(WaitSecond(LobbyButton));
        OnLobbyButtonClicked?.Invoke();
    }
    
    public void OnClickSetting(){
        LobbyButton.interactable = false;   
        StartCoroutine(WaitSecond(LobbyButton));
        OnSettingButtonClicked?.Invoke();
    }
    
    public void OnClickQuit(){
        LobbyButton.interactable = false;   
        StartCoroutine(WaitSecond(LobbyButton));
        OnQuitButtonClicked?.Invoke();
    }
    
    public void OnTest(){
        LobbyButton.interactable = false;   
        StartCoroutine(WaitSecond(LobbyButton));
        OnTestButtonClicked?.Invoke();
    }
    
}
