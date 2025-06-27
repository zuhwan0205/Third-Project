using System;
using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Fusion.Photon.Realtime;

public class UIManager : MonoBehaviour
{
    private NetworkRunner runner;
    [SerializeField] private GameObject SettingPanel;
    private bool isShutdownComplete = false;
    

    void OnEnable()
    {
        Event_MainScene.OnLobbyButtonClicked += LobbyButton;
        Event_MainScene.OnSettingButtonClicked += SettingButton;
        Event_MainScene.OnQuitButtonClicked += QuitButton;
        Event_MainScene.OnTestButtonClicked += testButton;
        Event_MainScene.OnCloseSettingButtonClicked += CloseSetting;
    }

    void OnDisable()
    {
        Event_MainScene.OnLobbyButtonClicked -= LobbyButton;
        Event_MainScene.OnSettingButtonClicked -= SettingButton;
        Event_MainScene.OnQuitButtonClicked -= QuitButton;
        Event_MainScene.OnTestButtonClicked -= testButton;
        Event_MainScene.OnCloseSettingButtonClicked -= CloseSetting;
    }

    private async void LobbyButton()
    {
        
    }
    
    private void SettingButton()
    {
        SettingPanel.SetActive(true);
    }
    
    private void QuitButton()
    {
        
    }

    private void testButton()
    {
        
    }

    private void CloseSetting()
    {
        SettingPanel.SetActive(false);
    }
    
    
}
