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
        runner = FindFirstObjectByType<NetworkRunner>();
        
        if (runner == null)
        {
            Debug.LogError("[LobbyUIManager] NetworkRunner not found!");
            return;
        }

        bool isHost = runner.IsServer;
        
        startButton.gameObject.SetActive(isHost);
        readyButton.gameObject.SetActive(!isHost);
        
        if (isHost)
        {
            startButton.interactable = false;
        }
    }

    public void OnReadyClicked()
    {
        var lobbyManager = FindFirstObjectByType<LobbyManager>();
        
        if (lobbyManager != null)
        {
            lobbyManager.RPC_SetReady(runner.LocalPlayer);
            readyButton.interactable = false;
            readyButton.GetComponentInChildren<TMPro.TMP_Text>().text = "Ready!";
        }
        else
        {
            Debug.LogError("[LobbyUIManager] LobbyManager not found!");
        }
    }

    public void EnableStartButton(bool enable)
    {
        if (runner != null && runner.IsServer)
        {
            startButton.interactable = enable;
        }
    }

    public void OnStartClicked()
    {
        if (runner != null && runner.IsServer)
        {
            runner.LoadScene("GameScene");
            startButton.interactable = false;
        }
        else
        {
            Debug.LogError("[LobbyUIManager] Only host can start the game!");
        }
    }
}
