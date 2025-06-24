using System;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;

public class NetworkRunnerHandler : MonoBehaviour, INetworkRunnerCallbacks
{
    private static NetworkRunnerHandler instance;
    private NetworkRunner runner;

    public static NetworkRunnerHandler Instance => instance;
    
    public static event Action<PlayerRef> OnLobbyPlayerJoined;
    public static event Action<PlayerRef> OnLobbyPlayerLeft;
    public static event Action OnLobbySceneLoadDone;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        if(runner == null) runner = gameObject.AddComponent<NetworkRunner>();
        runner.ProvideInput = true;
        runner.AddCallbacks(this);
    }

    public NetworkRunner GetRunner()
    {
        return runner;
    }
    
    public void SetRunner(NetworkRunner newRunner)
    {
        runner = newRunner;
    }

    public void OnInput(NetworkRunner runner, NetworkInput input) 
    { 
        var data = new NetworkInputData();
        data.MovementInput.x = Input.GetAxis("Horizontal");
        data.MovementInput.y = Input.GetAxis("Vertical");
        
        data.IsJumping = Input.GetKey(KeyCode.Space);
        
        input.Set(data);
        
    }
    
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        OnLobbyPlayerJoined?.Invoke(player);
    }
    
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) 
    {
        OnLobbyPlayerLeft?.Invoke(player);
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        string currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        
        if (currentSceneName == "LobbyScene")
        {
            OnLobbySceneLoadDone?.Invoke();
        }
        
        if (currentSceneName == "LeeScene" && runner.IsServer)
        {
            
            var gameManagerObj = FindFirstObjectByType<GameManager>();
            if (gameManagerObj != null && gameManagerObj.Object == null)
            {
                var networkObject = gameManagerObj.GetComponent<NetworkObject>();
                if (networkObject != null)
                {
                    runner.Spawn(networkObject);
                }
            }
            else if (gameManagerObj == null)
            {
            }
        }
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        this.runner = null;
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
    }

    // 나머지 콜백들 (빈 구현)
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, System.ArraySegment<byte> data) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
}
public struct NetworkInputData : INetworkInput
{
    public Vector2 MovementInput;
    public bool IsJumping;
    public NetworkButtons Buttons;
}
