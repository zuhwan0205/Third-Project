using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Fusion;

public class GameScene_PlayerUi : MonoBehaviour
{   
    [SerializeField] private Slider playerHp;
    [SerializeField] private Slider playerHunger;
    [SerializeField] private TMP_Text playerNameText;
    
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
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
