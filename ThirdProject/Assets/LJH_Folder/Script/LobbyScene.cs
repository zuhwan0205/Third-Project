using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Fusion;

public class LobbyScene : MonoBehaviour
{
    public Transform memberListParent;
    public GameObject memberItemPrefab;

    private NetworkRunner runner;
    private List<PlayerRef> currentPlayerList = new List<PlayerRef>();
    
    void OnEnable()
    {
        NetworkRunnerHandler.OnLobbyPlayerJoined += HandlePlayerJoined;
        NetworkRunnerHandler.OnLobbyPlayerLeft += HandlePlayerLeft;
        NetworkRunnerHandler.OnLobbySceneLoadDone += HandleSceneLoadDone;
    }

    void OnDisable()
    {
        NetworkRunnerHandler.OnLobbyPlayerJoined -= HandlePlayerJoined;
        NetworkRunnerHandler.OnLobbyPlayerLeft -= HandlePlayerLeft;
        NetworkRunnerHandler.OnLobbySceneLoadDone -= HandleSceneLoadDone;
    }

    void Start()
    {

        runner = FindFirstObjectByType<NetworkRunner>();
        if (runner != null)
        {
            currentPlayerList = runner.ActivePlayers.ToList();
            UpdateMemberListUI();
        }
        else
        {
            Debug.LogError("[LobbyScene] NetworkRunner를 씬에서 찾을 수 없음");
        }
    }

    private void HandlePlayerJoined(PlayerRef player)
    {
        if (runner == null) return;
        
        currentPlayerList = runner.ActivePlayers.ToList();
        UpdateMemberListUI();
    }

    private void HandlePlayerLeft(PlayerRef player)
    {
        if (runner == null) return;
        
        currentPlayerList = runner.ActivePlayers.ToList();
        UpdateMemberListUI();
    }

    private void HandleSceneLoadDone()
    {
        if (runner == null) return;
        currentPlayerList = runner.ActivePlayers.ToList();
        UpdateMemberListUI();
    }

    private void UpdateMemberListUI()
    {
        if (memberListParent == null)
        {
            Debug.LogWarning("[LobbyScene] memberListParent is null.");
            return;
        }
        
        foreach (Transform child in memberListParent)
        {
            Destroy(child.gameObject);
        }
        
        foreach (var player in currentPlayerList)
        {
            if (memberItemPrefab != null && memberListParent != null)
            {
                GameObject memberItem = Instantiate(memberItemPrefab, memberListParent);
                var memberItemComponent = memberItem.GetComponent<MemberItem>();
                if (memberItemComponent != null)
                {
                    memberItemComponent.Setup(runner, player);
                }
            }
        }
    }
    
    /*public void QuitToMain()
    {
        Debug.Log("[LobbyScene] Quit to MainScene");
        SceneManager.LoadScene("MainScene");
    }*/
}