using System;
using UnityEngine;
using UnityEngine;
using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager Instance { get; private set; }
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
            Debug.Log("Photon 서버에 연결 시도 중...");
        }
    }
    
    public override void OnConnectedToMaster()
    {
        Debug.Log("NetworkManager : 포톤 마스터 서버 접속 완료");
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }
    }

    public override void OnJoinedLobby()
    {
        PhotonNetwork.LoadLevel("MainScene");
    }
}
