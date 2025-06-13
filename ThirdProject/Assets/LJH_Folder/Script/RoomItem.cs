using UnityEngine;
using UnityEngine.UI;
using Fusion;
using TMPro;

public class RoomItem : MonoBehaviour
{
    public TMP_Text roomNameText;
    public TMP_Text playerCountText;
    private string roomName;
    private System.Action<string> onClickCallback;

    public void Setup(SessionInfo session, System.Action<string> onClick)
    {
        roomName = session.Name;
        roomNameText.text = roomName;
        playerCountText.text = $"{session.PlayerCount}/{session.MaxPlayers}";
        onClickCallback = onClick;
    }

    public void OnClick()
    {
        onClickCallback?.Invoke(roomName);
    }
}