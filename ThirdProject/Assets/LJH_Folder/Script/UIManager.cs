using System;
using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

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
    }

    void OnDisable()
    {
        Event_MainScene.OnLobbyButtonClicked -= LobbyButton;
        Event_MainScene.OnSettingButtonClicked -= SettingButton;
        Event_MainScene.OnQuitButtonClicked -= QuitButton;
        Event_MainScene.OnTestButtonClicked += testButton;
    }

    private void Start()
    {
        runner = NetworkRunnerHandler.Instance.GetRunner();
    }

    private async void LobbyButton()
    {
        StartCoroutine(StartHostAndLoadLobby());
    }
    
    private void SettingButton()
    {
        SceneManager.LoadScene("JDHScene");
    }
    
    private void QuitButton()
    {
        SceneManager.LoadScene("KKI_TestScene");
    }

    private void testButton()
    {
        SceneManager.LoadScene("Inventory");
    }
    
    IEnumerator StartHostAndLoadLobby()
    {
        Debug.Log("[LoadingScene] Starting as Host → Create + Join GlobalLobby.");
        
        int lobbySceneBuildIndex = SceneUtility.GetBuildIndexByScenePath("Assets/Scenes/LobbyScene.unity"); // 또는 직접 숫자
        var scene = SceneRef.FromIndex(lobbySceneBuildIndex);
        var sceneInfo = new NetworkSceneInfo();

        var startGameTask = runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.AutoHostOrClient,
            SessionName = "GameLobby",
            Scene = scene,
            SceneManager = runner.gameObject.GetComponent<NetworkSceneManagerDefault>() ?? runner.gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
        

        yield return new WaitUntil(() => runner.IsRunning);

        Debug.Log("[LoadingScene] Host started → MainScene 이동");

        yield return new WaitForSeconds(1f);
        //SceneManager.LoadScene("LobbyScene");
    }
}
