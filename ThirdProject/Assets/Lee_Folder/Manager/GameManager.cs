using UnityEngine;
using Fusion;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

public enum GameState
{
    WaitingForPlayers,
    AllPlayersLoaded,
    SpawningPlayers,
    GameStarted
}

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;
    
    [Header("Game State")]
    [Networked] public GameState CurrentState { get; set; } = GameState.WaitingForPlayers;
    [Networked] public int ExpectedPlayers { get; set; } = 0;
    [Networked] public int LoadedPlayers { get; set; } = 0;
    
    [Networked, Capacity(6)]
    public NetworkDictionary<PlayerRef, bool> PlayerLoadedStates => default;
    
    private void Awake()
    {
        Instance = this;
    }
    
    public override void Spawned()
    {
        if (!Runner.IsServer) return;
        
        StartCoroutine(WaitForAllPlayersToLoad());
    }
    
    IEnumerator WaitForAllPlayersToLoad()
    {
        ExpectedPlayers = Runner.ActivePlayers.Count();
        CurrentState = GameState.WaitingForPlayers;
        
        
        yield return new WaitUntil(() => AreAllPlayersLoaded());
        
        CurrentState = GameState.AllPlayersLoaded;
        
        yield return new WaitForSeconds(1f);
        
        StartSpawnProcess();
    }
    
    private bool AreAllPlayersLoaded()
    {
        var activePlayers = Runner.ActivePlayers.ToList();
        
        if (activePlayers.Count != ExpectedPlayers)
        {
            return false;
        }
        
        foreach (var player in activePlayers)
        {
            if (!PlayerLoadedStates.ContainsKey(player) || !PlayerLoadedStates[player])
            {
                return false;
            }
        }
        
        return true;
    }
    
    private void StartSpawnProcess()
    {
        if (!Runner.IsServer) return;
        
        CurrentState = GameState.SpawningPlayers;
        
        if (SpawnManager.Instance != null)
        {
            SpawnManager.Instance.SpawnAllPlayers(Runner);
            
            StartCoroutine(CompleteGameStart());
        }
        else
        {
            Debug.LogError("[GameManager] SpawnManager instance not found!");
        }
    }
    
    IEnumerator CompleteGameStart()
    {
        yield return new WaitForSeconds(2f);
        
        CurrentState = GameState.GameStarted;
        
        RPC_NotifyGameStarted();
    }
    
    [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
    public void RPC_NotifyGameStarted()
    {
        OnGameStarted();
    }
    
    [Rpc(sources: RpcSources.All, targets: RpcTargets.StateAuthority)]
    public void RPC_PlayerLoadedIntoScene(PlayerRef player)
    {
        PlayerLoadedStates.Set(player, true);
        LoadedPlayers = PlayerLoadedStates.Count(kvp => kvp.Value);
    }
    
    private void OnGameStarted()
    {
        // 여기에 게임 시작 로직 추가
        // - UI 활성화
        // - 게임 타이머 시작
        // - 플레이어 입력 활성화 등
    }
    
    public void OnPlayerLeft(PlayerRef player)
    {
        if (!Runner.IsServer) return;
        
        if (PlayerLoadedStates.ContainsKey(player))
        {
            PlayerLoadedStates.Remove(player);
            LoadedPlayers = PlayerLoadedStates.Count(kvp => kvp.Value);
            ExpectedPlayers = Runner.ActivePlayers.Count();
            
        }
    }
    
    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}