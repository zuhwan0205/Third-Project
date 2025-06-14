using UnityEngine;
using UnityEngine.UI;
using Fusion;

public class LobbyUIManager : MonoBehaviour
{
    public static LobbyUIManager Instance;

    public Button startButton;
    public Button readyButton;

    private NetworkRunner runner;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        runner = FindObjectOfType<NetworkRunner>();

        bool isHost = runner.IsServer;

        startButton.gameObject.SetActive(isHost);   // 호스트만 Start 버튼
        readyButton.gameObject.SetActive(!isHost);  // 클라이언트만 Ready 버튼
    }

    public void OnReadyClicked()
    {
        var lobbyManager = FindObjectOfType<LobbyManager>();
        if (lobbyManager != null)
        {
            lobbyManager.RPC_SetReady(runner.LocalPlayer);
            readyButton.interactable = false;
        }
    }

    public void EnableStartButton(bool enable)
    {
        if (runner.IsServer)
            startButton.interactable = enable;
    }

    public void OnStartClicked()
    {
        if (runner.IsServer)
        {
            runner.LoadScene("GameScene");
        }
    }
}
