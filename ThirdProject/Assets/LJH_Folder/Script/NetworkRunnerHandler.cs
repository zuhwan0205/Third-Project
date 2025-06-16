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
    
    

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        Debug.Log($"[Lobby] OnShutdown called → Reason: {shutdownReason}");
        this.runner = null;
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        Debug.Log("[NetworkRunnerHandler] Connected to server.");
    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
        Debug.Log("[NetworkRunnerHandler] Disconnected from server.");
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        
    }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
        
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
        
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, System.ArraySegment<byte> data) { }
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        if (!runner.IsServer) return;
        Debug.Log("NetworkRunnerHandelr : OnSceneLoadDone");
        if (SpawnManager.Instance != null)
        {
            SpawnManager.Instance.SpawnAllPlayers(runner);
        }
        else
        {
            Debug.LogWarning("SpawnManager not found Instance");
        }
    }
    public void OnSceneLoadStart(NetworkRunner runner) { }
}
