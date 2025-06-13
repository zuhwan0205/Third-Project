using UnityEngine;
using UnityEngine.UI;
using Fusion;
using TMPro;

public class MemberItem : MonoBehaviour
{
    public TMP_Text playerNameText;

    public void Setup(PlayerRef player)
    {
        playerNameText.text = $"Player {player.RawEncoded}"; 
    }
}