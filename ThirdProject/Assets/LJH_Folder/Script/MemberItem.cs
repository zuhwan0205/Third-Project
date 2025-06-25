using UnityEngine;
using UnityEngine.UI;
using Fusion;
using TMPro;

public class MemberItem : MonoBehaviour
{
    public TMP_Text playerNameText;
    private NetworkRunner _runner;
    private PlayerRef     _player;
    
    public void Setup(NetworkRunner runner, PlayerRef player)
    {
        _runner = runner;
        _player = player;

        // PlayerRef로부터 UserID(=닉네임) 가져오기
        string userId = _runner.GetPlayerUserId(_player);
        playerNameText.text = userId;
    }
}