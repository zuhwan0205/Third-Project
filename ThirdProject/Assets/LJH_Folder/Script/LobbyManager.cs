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

    public override void Spawned()
    {
        
        if (Runner != null && Runner.IsServer)
        {
            readyStates[Runner.LocalPlayer] = true;
            CheckAllReady();
        }
    }
    
    [Rpc(sources: RpcSources.All, targets: RpcTargets.StateAuthority)]
    public void RPC_SetReady(PlayerRef player)
    {
        readyStates[player] = true;
        
        CheckAllReady();
    }

    private void CheckAllReady()
    {
        if (AllReady())
        {
            if (LobbyUIManager.Instance != null)
            {
                LobbyUIManager.Instance.EnableStartButton(true);
            }
        }
        else
        {
            Debug.Log($"[LobbyManager] Waiting for more players. Ready: {readyStates.Count(kv => kv.Value)}/{Runner.ActivePlayers.Count()}");
        }
    }

    private bool AllReady()
    {
        var activePlayers = Runner.ActivePlayers.ToList();
        
        if (activePlayers.Count == 0)
            return false;
        
        bool allReady = activePlayers.All(player => 
            readyStates.ContainsKey(player) && readyStates[player]);
        
        
        return allReady;
    }
    
    public void OnPlayerLeft(PlayerRef player)
    {
        if (readyStates.ContainsKey(player))
        {
            readyStates.Remove(player);
            CheckAllReady();
        }
    }
}
