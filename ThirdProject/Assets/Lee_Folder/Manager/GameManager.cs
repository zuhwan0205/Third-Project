using System;
using Photon.Pun;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState
    {
        Lobby,
        Waiting,
        Playing,
        GameOver
    }

    public GameState CurrentState { get; private set; } = GameState.Lobby;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SetGameState(GameState.Waiting);
        }
    }

    /// <summary>
    /// 현재 게임의 상태를 변경하는 로직(GameManager)
    /// 사용법 : SetGameState(GameState타입 파라미터)
    /// </summary>
    /// <param name="newState"></param>
    public void SetGameState(GameState newState)
    {
        CurrentState = newState;

        if (newState == GameState.Waiting)
        {
            Debug.Log("[GameManager] 상태 전환: Waiting");
            SpawnManager.Instance.AssignAllSpawnPositions();
        }
    }
}
