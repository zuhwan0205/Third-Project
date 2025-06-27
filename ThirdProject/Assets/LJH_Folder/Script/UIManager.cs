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
    [SerializeField] private GameObject StartPanel;
    [SerializeField] GameObject AudioManager;
    private bool isShutdownComplete = false;
    

    void OnEnable()
    {
        Event_MainScene.OnLobbyButtonClicked += LobbyButton;
        Event_MainScene.OnSettingButtonClicked += SettingButton;
        Event_MainScene.OnQuitButtonClicked += QuitButton;
        Event_MainScene.OnTestButtonClicked += testButton;
        Event_MainScene.OnCloseSettingButtonClicked += CloseSetting;
        Event_MainScene.OnNormalButtonClicked += NormalMode;
        Event_MainScene.OnHardButtonClicked += HardMode;
    }

    void OnDisable()
    {
        Event_MainScene.OnLobbyButtonClicked -= LobbyButton;
        Event_MainScene.OnSettingButtonClicked -= SettingButton;
        Event_MainScene.OnQuitButtonClicked -= QuitButton;
        Event_MainScene.OnTestButtonClicked -= testButton;
        Event_MainScene.OnCloseSettingButtonClicked -= CloseSetting;
        Event_MainScene.OnNormalButtonClicked -= NormalMode;
        Event_MainScene.OnHardButtonClicked -= HardMode;
    }

    private void LobbyButton()
    {
        StartPanel.SetActive(true);
    }
    
    private void SettingButton()
    {
        SettingPanel.SetActive(true);
    }
    
    private void QuitButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    private void testButton()
    {
        
    }

    private void CloseSetting()
    {
        SettingPanel.SetActive(false);
    }

    private void NormalMode()
    {
        if (AudioManager != null)
        {
            Destroy(AudioManager_Main.instance.gameObject);
        }
        SceneManager.LoadScene("GameScene");
    }

    private void HardMode()
    {
        if (AudioManager != null)
        {
            Destroy(AudioManager_Main.instance.gameObject);
        }
        SceneManager.LoadScene("GameScene");
    }
    
    
}
