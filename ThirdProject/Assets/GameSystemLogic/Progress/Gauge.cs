using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Gauge : MonoBehaviour
{
    public static Gauge Instance { get; private set; }

    [Header("Gauge UI")]
    [SerializeField] private Slider gaugeSlider;
    
    [Header("Gauge Settings")]
    [SerializeField] private float maxGauge = 200f;
    [SerializeField] private float currentGauge = 0f;
    
    [Header("Ending")]
    [SerializeField] private EndingTextBank endingTextBank; // 엔딩 스크립터블 오브젝트
    

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        InitializeGauge();
    }

    private void InitializeGauge()
    {
        currentGauge = 0f;
        UpdateGaugeDisplay();
        
        Debug.Log($"게이지 초기화: {currentGauge}/{maxGauge}");
    }
    
    public bool TryAddGauge(float amount)
    {
        float targetGauge = currentGauge + amount;
        
        if (targetGauge < 0f)
        {
            ShowInsufficientGaugeMessage();
            return false;
        }
        
        float previousGauge = currentGauge;
        currentGauge = targetGauge;
        currentGauge = Mathf.Clamp(currentGauge, 0f, maxGauge);
        
        UpdateGaugeDisplay();
        
        if (currentGauge >= maxGauge)
        {
            OnGaugeFull();
        }
        
        if (currentGauge <= 0f)
        {
            OnGaugeEmpty();
        }
        
        return true;
    }
    
    public void AddGauge(float amount)
    {
        TryAddGauge(amount);
    }
    
    // 타임아웃 시 강제로 게이지를 깎는 메서드 (게이지 부족 체크 무시)
    public void ForceReduceGauge(float amount)
    {
        currentGauge -= amount;
        currentGauge = Mathf.Clamp(currentGauge, 0f, maxGauge);
        
        UpdateGaugeDisplay();
        
        if (currentGauge <= 0f)
        {
            OnGaugeEmpty();
        }
        
        Debug.Log($"강제 게이지 감소: -{amount}, 현재 게이지: {currentGauge}");
    }
    
    private void ShowInsufficientGaugeMessage()
    {
        string message = $"현재 게이지 수치는 {currentGauge:F0}로 게이지가 부족합니다.";
        Debug.LogWarning(message);
        
        if (QuestionManager.Instance != null)
        {
            QuestionManager.Instance.OnGaugeInsufficient(message);
        }
    }

    private void UpdateGaugeDisplay()
    {
        if (gaugeSlider != null)
        {
            float normalizedValue = currentGauge / maxGauge;
            gaugeSlider.value = normalizedValue;
        }
        
    }

    private void OnGaugeFull()
    {
        // 타이머 정지
        if (Timer.Instance != null)
        {
            Timer.Instance.StopTimer();
        }
    
        // QuestionManager의 모든 Invoke 취소 및 엔딩 모드 설정
        if (QuestionManager.Instance != null)
        {
            QuestionManager.Instance.CancelAllInvokes(); // 이 메서드를 QuestionManager에 추가해야 함
            QuestionManager.Instance.SetEndingMode(true); // 이 메서드도 추가해야 함
            QuestionManager.Instance.StartEndingSequence(endingTextBank);
        }
        else
        {
            // QuestionManager가 없으면 바로 환경 효과 시작
            StartEndingEnvironmentEffect();
        }
    }

    public void StartEndingEnvironmentEffect()
    {
        if (EnvironmentManager.Instance != null)
        {
            EnvironmentManager.Instance.StartEndingSequence();
        }
        else
        {
            // EnvironmentManager가 없으면 바로 엔딩 씬으로
            SceneManager.LoadScene("EndingScene");
        }
    }

    private void OnGaugeEmpty()
    {
        
    }
    
    public float GetCurrentGauge()
    {
        return currentGauge;
    }
    
    public float GetGaugeRatio()
    {
        return currentGauge / maxGauge;
    }
    
    public void ResetGauge()
    {
        currentGauge = 0f;
        UpdateGaugeDisplay();
    }
    
    public void SetGauge(float value)
    {
        currentGauge = Mathf.Clamp(value, 0f, maxGauge);
        UpdateGaugeDisplay();
    }
    
    public void SetMaxGauge(float newMaxGauge)
    {
        maxGauge = newMaxGauge;
        currentGauge = Mathf.Clamp(currentGauge, 0f, maxGauge);
        UpdateGaugeDisplay();
    }
}