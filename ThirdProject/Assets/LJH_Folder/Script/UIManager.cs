using System;
using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class UIManager : MonoBehaviour
{
    private NetworkRunner runner;
    [SerializeField] private GameObject SettingPanel;
    
    

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

    private void Start()
    {
        runner = NetworkRunnerHandler.Instance.GetRunner();
    }

    private void LobbyButton()
    {
        if (runner != null && runner.IsRunning)
        {
            runner.Shutdown();
        }
        StartCoroutine(StartHostAndLoadLobby());
    }
    
    private void SettingButton()
    {
        StartCoroutine(test1());
        //SettingPanel.SetActive(true);
    }
    
    private void QuitButton()
    {
        StartCoroutine(test2());
    }

    private void testButton()
    {
        StartCoroutine(test3());
    }

    private void CloseSetting()
    {
        SettingPanel.SetActive(false);
    }
    
    IEnumerator StartHostAndLoadLobby()
    {
        
        int lobbySceneBuildIndex = SceneUtility.GetBuildIndexByScenePath("Assets/Scenes/LobbyScene.unity"); // 또는 직접 숫자
        var scene = SceneRef.FromIndex(lobbySceneBuildIndex);

        var startGameArgs = new StartGameArgs()
        {
            GameMode = GameMode.AutoHostOrClient,
            SessionName = "GameLobby",
            Scene = scene,
            SceneManager = runner.gameObject.GetComponent<NetworkSceneManagerDefault>() ?? runner.gameObject.AddComponent<NetworkSceneManagerDefault>()
        };
        
        var startGameTask = runner.StartGame(startGameArgs);

        yield return new WaitUntil(() => runner.IsRunning);

        yield return new WaitForSeconds(0.5f);
        //SceneManager.LoadScene("LobbyScene");
    }
    
    IEnumerator test1()
    {
        if (runner != null && runner.IsRunning)
        {
            runner.Shutdown();
            yield return new WaitForSeconds(0.5f); // 종료 대기
        }
        
        int sceneBuildIndex = SceneUtility.GetBuildIndexByScenePath("Assets/Scenes/JDHScene.unity");
        var scene = SceneRef.FromIndex(sceneBuildIndex);

        var startGameTask = runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.AutoHostOrClient,
            SessionName = "JDH_Test",
            Scene = scene,
            SceneManager = runner.gameObject.GetComponent<NetworkSceneManagerDefault>() ?? 
                           runner.gameObject.AddComponent<NetworkSceneManagerDefault>()
        });

        yield return new WaitUntil(() => runner.IsRunning);
        //SceneManager.LoadScene("LobbyScene");
    }
    
    IEnumerator test2()
    {
        
        if (runner != null && runner.IsRunning)
        {
            runner.Shutdown();
            yield return new WaitForSeconds(0.5f); // 종료 대기
        }
        
        int sceneBuildIndex = SceneUtility.GetBuildIndexByScenePath("Assets/Scenes/KKI_TestScene.unity");
        var scene = SceneRef.FromIndex(sceneBuildIndex);

        var startGameTask = runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.AutoHostOrClient,
            SessionName = "KKI_Test",
            Scene = scene,
            SceneManager = runner.gameObject.GetComponent<NetworkSceneManagerDefault>() ?? 
                           runner.gameObject.AddComponent<NetworkSceneManagerDefault>()
        });

        yield return new WaitUntil(() => runner.IsRunning);
        //SceneManager.LoadScene("LobbyScene");
    }
    
    IEnumerator test3()
    {
        
        if (runner != null && runner.IsRunning)
        {
            runner.Shutdown();
            yield return new WaitForSeconds(0.5f); // 종료 대기
        }
        
        int sceneBuildIndex = SceneUtility.GetBuildIndexByScenePath("Assets/Scenes/KYW_Inventory.unity");
        var scene = SceneRef.FromIndex(sceneBuildIndex);

        var startGameTask = runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.AutoHostOrClient,
            SessionName = "KYW_Test",
            Scene = scene,
            SceneManager = runner.gameObject.GetComponent<NetworkSceneManagerDefault>() ?? 
                           runner.gameObject.AddComponent<NetworkSceneManagerDefault>()
        });

        yield return new WaitUntil(() => runner.IsRunning);
        //SceneManager.LoadScene("LobbyScene");
    }
}
