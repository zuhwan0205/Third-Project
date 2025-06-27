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
    public bool IsInputAllowed { get; private set; } = false;
    
    [Header("Game State")]
    [Networked] public GameState CurrentState { get; set; } = GameState.WaitingForPlayers;
    [Networked] public int ExpectedPlayers { get; set; } = 0;
    [Networked] public int LoadedPlayers { get; set; } = 0;
    
    [Networked, Capacity(9)]
    public NetworkDictionary<PlayerRef, bool> PlayerLoadedStates => default;
    
    private void Awake()
    {
        Instance = this;
    }
    
    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
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
    
    private void OnGameStarted()
    {
        RoomManager.Instance?.StartIntroSequence();
    }
    
    public void OnGameIntroComplete()
    {
        IsInputAllowed = true;
        
        for (int i = 0; i < RoomManager.Instance.RoomCount; i++)
        {
            RoomManager.Instance.SetRoomAlive(i, true);
        }
        
        if (Object.HasStateAuthority)
        {
            RoomManager.Instance?.StartSurvivalTest();
        }
    }
    
    public void CheckNextQuestionTrigger()
    {
        if (!Runner.IsServer)
        {
            return;
        }

        int aliveCount = 0;
        int answeredCount = 0;

        for (int i = 0; i < RoomManager.Instance.RoomCount; i++)
        {
            var room = RoomManager.Instance.GetRoom(i);
            if (room == null) continue;

            if (!room.state.isAlive)
            {
                Debug.Log($"Room {i} isDead → Skip");
                continue;
            }

            aliveCount++;
            Debug.Log($"Room {i} - hasAnswered: {room.state.hasAnswered}");

            if (room.state.hasAnswered)
            {
                answeredCount++;
            }
            else
            {
                return;
            }
        }
        
        Debug.Log($"AlivePlayers ({answeredCount}/{aliveCount}) → StartNextQuestion");
        RoomManager.Instance?.StartNextQuestion();
    }

    public void OnQuestionPhaseComplete(RoomStateData[] roomStates)
    {
        if (!Object.HasStateAuthority) return;
        
        int[] roomIndices = new int[roomStates.Length];
        bool[] aliveStates = new bool[roomStates.Length];
        bool[] answeredStates = new bool[roomStates.Length];
        
        for (int i = 0; i < roomStates.Length; i++)
        {
            roomIndices[i] = roomStates[i].roomIndex;
            aliveStates[i] = roomStates[i].isAlive;
            answeredStates[i] = roomStates[i].hasAnswered;
        }
    
        RPC_SyncAllRoomStates(roomIndices, aliveStates, answeredStates);
        CheckNextQuestionTrigger();
    }
    
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_SyncAllRoomStates(int[] roomIndices, bool[] aliveStates, bool[] answeredStates)
    {
    
        for (int i = 0; i < roomIndices.Length; i++)
        {
            RoomManager.Instance?.SetRoomAlive(roomIndices[i], aliveStates[i]);
            var room = RoomManager.Instance?.GetRoom(roomIndices[i]);
            if (room != null)
            {
                room.state.hasAnswered = answeredStates[i];
            }
        }
    }
    
    [Rpc(sources: RpcSources.All, targets: RpcTargets.StateAuthority)]
    public void RPC_PlayerLoadedIntoScene(PlayerRef player)
    {
        PlayerLoadedStates.Set(player, true);
        LoadedPlayers = PlayerLoadedStates.Count(kvp => kvp.Value);
    }
    
    
    // 지금은 아직 못씀
    /*public void HandleQuestionEffect(RoomQuestion question)
    {
        switch (question.type)
        {
            case QuestionType.Positive:
                //TrySpawnRewardItems(question.positiveRewards);
                break;

            case QuestionType.Negative:
                //TrySpawnMonsters(question.monsterList);
                break;

            case QuestionType.Neutral:
            default:
                break;
        }
    }*/
    
}