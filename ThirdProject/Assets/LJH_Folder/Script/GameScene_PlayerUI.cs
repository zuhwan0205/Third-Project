using System;
using TMPro;
using UnityEngine;
using Fusion;
using UnityEngine.UI;

public class GameScene_PlayerUI : MonoBehaviour
{
    [SerializeField] TMP_Text playerNameText;
    [SerializeField] Slider healthSlider;
    [SerializeField] Slider hungerSlider;
    
    private NetworkRunner _runner;
    private PlayerRef     _player;
    private PlayerController _pc;
    
    private float _maxHealth, _maxHunger;

    public void Initialize(float maxHp, float maxHungry, PlayerController pc) {
        _maxHealth = maxHp;
        _maxHunger = maxHungry;
        healthSlider.maxValue = maxHp;
        hungerSlider.maxValue = maxHungry;
    }

    // 매 프레임 호출되는 함수
    public void SetHealth(float hp) {
        healthSlider.value = hp;
    }
    public void SetHunger(float hungry) {
        hungerSlider.value = hungry;
    }

    private void Start()
    {
        playerNameText.text = SessionData.Nickname;
    }
}
