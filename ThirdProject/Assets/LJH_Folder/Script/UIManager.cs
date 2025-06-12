using Photon.Pun;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject SettingPanel;

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

    private void LobbyButton()
    {
        PhotonNetwork.LoadLevel("KKI_TestScene");
    }
    
    private void SettingButton()
    {
        PhotonNetwork.LoadLevel("JDHScene");
    }
    
    private void QuitButton()
    {
        PhotonNetwork.LoadLevel("LeeScene");
    }

    private void testButton()
    {
        PhotonNetwork.LoadLevel("Inventory");
    }
}
