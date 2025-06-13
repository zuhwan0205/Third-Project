using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;

public class PlayerListManager : NetworkBehaviour
{
    public static PlayerListManager Instance;

    void Awake()
    {
        Instance = this;
    }

    public override void Spawned()
    {
        Debug.Log("[PlayerListManager] Spawned → Broadcasting initial player list");

        // Host가 Spawn 시점에 전체 멤버 목록 한번 보내주기
        if (Runner.IsServer)
        {
            RPC_SendPlayerList(Runner.ActivePlayers.ToArray());
        }
    }

    public void OnPlayerListChanged()
    {
        // PlayerList 변경시 (PlayerJoined / PlayerLeft 등에서 호출)
        if (Runner.IsServer)
        {
            Debug.Log("[PlayerListManager] OnPlayerListChanged → Sending updated player list");
            RPC_SendPlayerList(Runner.ActivePlayers.ToArray());
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_SendPlayerList(PlayerRef[] players)
    {
        Debug.Log($"[PlayerListManager] RPC_SendPlayerList → players.Count={players.Length}");

        // LobbyScene에 전달
        if (LobbyScene.Instance != null)
        {
            LobbyScene.Instance.UpdateMemberListFromRPC(players);
        }
    }
}