using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

public class LobbyManager2 : MonoBehaviourPunCallbacks
{
    [Header("UI Elements")]
    [SerializeField] private TMP_InputField roomNameInput;
    [SerializeField] private TMP_InputField chatInput;
    [SerializeField] private TextMeshProUGUI chatText;
    [SerializeField] private Transform roomListContent;
    [SerializeField] private GameObject roomListItemPrefab;
    [SerializeField] private Button createRoomButton;
    [SerializeField] private Button joinRoomButton;

    private void Start()
    {
        createRoomButton.onClick.AddListener(CreateRoom);
        joinRoomButton.onClick.AddListener(JoinRoom);
    }

    // 방 생성
    private void CreateRoom()
    {
        if (string.IsNullOrEmpty(roomNameInput.text)) return;
        NetworkManager2.Instance.CreateRoom(roomNameInput.text);
    }

    // 방 참가
    private void JoinRoom()
    {
        if (string.IsNullOrEmpty(roomNameInput.text)) return;
        NetworkManager2.Instance.JoinRoom(roomNameInput.text);
    }

    // 채팅 메시지 전송
    public void SendChatMessage()
    {
        if (string.IsNullOrEmpty(chatInput.text)) return;

        string message = $"{PhotonNetwork.LocalPlayer.NickName}: {chatInput.text}";
        photonView.RPC("ReceiveChatMessage", RpcTarget.All, message);
        chatInput.text = "";
    }

    // 채팅 메시지 수신
    [PunRPC]
    private void ReceiveChatMessage(string message)
    {
        chatText.text += message + "\n";
    }

    // 방 목록 업데이트
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (Transform item in roomListContent)
        {
            Destroy(item.gameObject);
        }

        foreach (RoomInfo room in roomList)
        {
            if (room.RemovedFromList) continue;

            GameObject roomItem = Instantiate(roomListItemPrefab, roomListContent);
            roomItem.GetComponentInChildren<TextMeshProUGUI>().text = $"{room.Name} ({room.PlayerCount}/{room.MaxPlayers})";
            roomItem.GetComponent<Button>().onClick.AddListener(() => NetworkManager2.Instance.JoinRoom(room.Name));
        }
    }
} 