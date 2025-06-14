using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;

public class LobbyManager : NetworkBehaviour
{
    public static LobbyManager Instance;

    private Dictionary<PlayerRef, bool> readyStates = new Dictionary<PlayerRef, bool>();

    private void Awake()
    {
        Instance = this;
    }

    [Rpc(sources: RpcSources.All, targets: RpcTargets.StateAuthority)]
    public void RPC_SetReady(PlayerRef player)
    {
        Debug.Log($"[LobbyManager] Ready 받음: {player}");
        readyStates[player] = true;

        if (AllReady())
        {
            Debug.Log("[LobbyManager] 모든 플레이어가 Ready 상태");
            LobbyUIManager.Instance.EnableStartButton(true);
        }
    }

    private bool AllReady()
    {
        if (readyStates.Count != Runner.ActivePlayers.Count()-1)
            return false;

        return Runner.ActivePlayers.All(p => readyStates.ContainsKey(p) && readyStates[p]);
    }
}
