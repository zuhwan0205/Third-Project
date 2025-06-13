using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    private NetworkRunner runner;
    [SerializeField] private GameObject SettingPanel;
    private bool isShutdownComplete = false;
    

    void OnEnable()
    {
        runner = NetworkRunnerHandler.Instance.GetRunner();
        
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

    private async void LobbyButton()
    {
        SceneManager.LoadScene("LobbyScene");
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
}
