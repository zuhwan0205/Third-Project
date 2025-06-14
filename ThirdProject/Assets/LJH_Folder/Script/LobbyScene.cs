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

    public Transform memberListParent;
    public GameObject memberItemPrefab;

    private NetworkRunner runner;
    private List<PlayerRef> currentPlayerList = new List<PlayerRef>();
    
    void Awake()
    {
        Debug.Log(" [LobbyScene] Awake 호출됨");

        runner = FindObjectOfType<NetworkRunner>();
        if (runner != null)
        {
            runner.AddCallbacks(this);
            Debug.Log(" [LobbyScene] runner.AddCallbacks(this) 등록 완료");
        }
        else
        {
            Debug.LogError(" [LobbyScene] NetworkRunner를 씬에서 찾을 수 없음");
        }
    }

    private void UpdateMemberListUI()
    {
        Debug.Log($"[LobbyScene] UpdateMemberListUI 실행. currentPlayerList.Count: {currentPlayerList.Count}");
        
        if (memberListParent == null)
        {
            Debug.LogWarning("[LobbyScene] memberListParent is null. 씬이 언로드됐거나 오브젝트가 파괴됨.");
            return;
        }

        foreach (Transform child in memberListParent)
        {
            Destroy(child.gameObject);
        }

        StartCoroutine(Delay());

        foreach (var player in currentPlayerList)
        {
            GameObject memberItem = Instantiate(memberItemPrefab, memberListParent);
            memberItem.GetComponent<MemberItem>().Setup(player);
        }
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(1f);
    }
    
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"[Lobby] Player joined: {player} [My PlayerRef: {runner.LocalPlayer}] ActivePlayers.Count={runner.ActivePlayers.Count()}");
        currentPlayerList = runner.ActivePlayers.ToList();
        UpdateMemberListUI();
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"[Lobby] Player joined: {player} [My PlayerRef: {runner.LocalPlayer}] ActivePlayers.Count={runner.ActivePlayers.Count()}");
        currentPlayerList = runner.ActivePlayers.ToList();
        UpdateMemberListUI();
        
    }
    
    public void OnSceneLoadDone(NetworkRunner runner)
    {
        Debug.Log("[LobbyScene] OnSceneLoadDone 호출됨");

        currentPlayerList = runner.ActivePlayers.ToList();
        UpdateMemberListUI();
    }
    
    public void QuitToMain()
    {
        Debug.Log("[Lobby] Quit to MainScene");
        SceneManager.LoadScene("MainScene");
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
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
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, System.ArraySegment<byte> data) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
}
