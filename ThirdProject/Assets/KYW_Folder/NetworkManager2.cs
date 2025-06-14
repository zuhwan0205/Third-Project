using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

public class NetworkManager2 : MonoBehaviourPunCallbacks
{
    // 싱글톤 패턴 구현
    public static NetworkManager2 Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // 포톤 서버 연결
        PhotonNetwork.ConnectUsingSettings();
    }

    // 서버 연결 성공 시 호출
    public override void OnConnectedToMaster()
    {
        Debug.Log("서버 연결 성공");
        PhotonNetwork.JoinLobby();
    }

    // 로비 입장 성공 시 호출
    public override void OnJoinedLobby()
    {
        Debug.Log("로비 입장 성공");
    }

    // 방 생성 함수
    public void CreateRoom(string roomName, int maxPlayers = 4)
    {
        RoomOptions options = new RoomOptions
        {
            MaxPlayers = (byte)maxPlayers,
            IsVisible = true,
            IsOpen = true
        };

        PhotonNetwork.CreateRoom(roomName, options);
    }

    // 방 참가 함수
    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    // 방 퇴장 함수
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    // 방 생성 성공 시 호출
    public override void OnCreatedRoom()
    {
        Debug.Log("방 생성 성공");
    }

    // 방 참가 성공 시 호출
    public override void OnJoinedRoom()
    {
        Debug.Log("방 참가 성공");
    }

    // 방 퇴장 성공 시 호출
    public override void OnLeftRoom()
    {
        Debug.Log("방 퇴장 성공");
    }
} 