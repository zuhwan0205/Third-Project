using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Fusion;
using Fusion.Sockets;

public class LobbyScene : MonoBehaviour, INetworkRunnerCallbacks
{
    public Button refreshButton;
    public Button quitButton;

    public NetworkObject playerListManagerPrefab;
    private NetworkObject playerListManagerInstance;

    public Transform memberListParent;
    public GameObject memberItemPrefab;

    private NetworkRunner runner;
    private List<PlayerRef> currentPlayerList = new List<PlayerRef>();

    public static LobbyScene Instance;

    void Awake()
    {
        Instance = this;

        if (NetworkRunnerHandler.Instance.GetRunner() != null && NetworkRunnerHandler.Instance.GetRunner().IsRunning)
        {
            Debug.Log("[Lobby] Awake → Shutting down previous Runner");
            NetworkRunnerHandler.Instance.GetRunner().Shutdown();
        }
    }

    IEnumerator Start()
    {
        yield return new WaitUntil(() => NetworkRunnerHandler.Instance.GetRunner() == null);
        yield return new WaitForSeconds(0.5f);

        Debug.Log("[Lobby] Creating new Runner");

        GameObject newRunnerGO = new GameObject("NetworkRunner");
        runner = newRunnerGO.AddComponent<NetworkRunner>();
        runner.ProvideInput = true;

        NetworkRunnerHandler.Instance.SetRunner(runner);
        runner.AddCallbacks(NetworkRunnerHandler.Instance);
        runner.AddCallbacks(this);

        runner.JoinSessionLobby(SessionLobby.ClientServer);
        Debug.Log("[Lobby] Joined Lobby → Waiting for SessionListUpdated...");

        UpdateMemberListUI();

        refreshButton.onClick.AddListener(() => runner.JoinSessionLobby(SessionLobby.ClientServer));
        quitButton.onClick.AddListener(QuitToMain);
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        Debug.Log($"[Lobby] OnSessionListUpdated → Found {sessionList.Count} sessions");

        // 바로 방 만들기 시도 (첫 진입 시 Host가 되도록)
        if (sessionList.Count == 0)
        {
            Debug.Log("[Lobby] No sessions → Creating new Host room");
            StartCoroutine(StartHostRoom());
        }
        else
        {
            Debug.Log("[Lobby] Existing session found → Joining as Client");
            StartCoroutine(StartJoinRoom(sessionList[0].Name));
        }
    }

    IEnumerator StartHostRoom()
    {
        var sessionName = "GameLobby_" + Guid.NewGuid().ToString();
        Debug.Log($"[Lobby] Will StartGame as Host with SessionName = {sessionName}");

        var startGameTask = runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Host,
            SessionName = sessionName,
            Scene = null,
            SceneManager = runner.gameObject.GetComponent<NetworkSceneManagerDefault>() ?? runner.gameObject.AddComponent<NetworkSceneManagerDefault>(),
            PlayerCount = 9
        });

        yield return startGameTask;
        yield return null;

        Debug.Log($"[Lobby] GameLobby Started → runner.IsServer={runner.IsServer}, SessionName={runner.SessionInfo.Name}, Players={runner.ActivePlayers.Count()}");

        if (runner.IsServer && playerListManagerPrefab != null)
        {
            playerListManagerInstance = runner.Spawn(playerListManagerPrefab, Vector3.zero, Quaternion.identity);
            Debug.Log("[Lobby] Spawned PlayerListManager");
        }

        UpdateMemberListUI();
    }

    IEnumerator StartJoinRoom(string roomName)
    {
        Debug.Log($"[Lobby] Will StartGame as Client, joining {roomName}");

        var startGameTask = runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Client,
            SessionName = roomName,
            Scene = null,
            SceneManager = runner.gameObject.GetComponent<NetworkSceneManagerDefault>() ?? runner.gameObject.AddComponent<NetworkSceneManagerDefault>(),
            PlayerCount = 9
        });

        yield return startGameTask;
        yield return null;

        Debug.Log($"[Lobby] Joined GameLobby → runner.IsServer={runner.IsServer}, SessionName={runner.SessionInfo.Name}, Players={runner.ActivePlayers.Count()}");

        UpdateMemberListUI();
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"[Lobby] Player joined: {player} [My PlayerRef: {runner.LocalPlayer}] ActivePlayers.Count={runner.ActivePlayers.Count()}");

        if (runner.IsServer && playerListManagerInstance != null)
        {
            Debug.Log("[Lobby] I am Host → Sending PlayerList to new player");
            PlayerListManager.Instance.RPC_SendPlayerList(runner.ActivePlayers.ToArray());
        }

        StartCoroutine(DelayedUpdateMemberListUI());
    }

    public void UpdateMemberListFromRPC(PlayerRef[] players)
    {
        currentPlayerList = new List<PlayerRef>(players);
        Debug.Log($"[Lobby] UpdateMemberListFromRPC → {players.Length} players");

        UpdateMemberListUI();
    }

    private void UpdateMemberListUI()
    {
        foreach (Transform child in memberListParent)
        {
            Destroy(child.gameObject);
        }

        foreach (var player in currentPlayerList)
        {
            GameObject memberItem = Instantiate(memberItemPrefab, memberListParent);
            memberItem.GetComponent<MemberItem>().Setup(player);
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"[Lobby] Player left: {player}");

        if (runner.IsServer && playerListManagerInstance != null)
        {
            Debug.Log("[Lobby] I am Host → Updating PlayerListManager");
            PlayerListManager.Instance.OnPlayerListChanged();
        }

        UpdateMemberListUI();
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        
    }

    public void QuitToMain()
    {
        Debug.Log("[Lobby] Quit to MainScene");
        SceneManager.LoadScene("MainScene");
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        Debug.Log("[Lobby] OnConnectRequest → Accepting player.");
        request.Accept();
    }

    // 나머지 INetworkRunnerCallbacks 빈 구현
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnConnectedToServer(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, System.ArraySegment<byte> data) { }

    IEnumerator DelayedUpdateMemberListUI()
    {
        yield return new WaitForSeconds(0.3f);
        Debug.Log($"[Lobby] DelayedUpdateMemberListUI → ActivePlayers={runner.ActivePlayers.Count()}");
        UpdateMemberListUI();
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
}
