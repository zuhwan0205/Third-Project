using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Gauge : MonoBehaviour
{
    public static Gauge Instance { get; private set; }

    [Header("Gauge UI")]
    [SerializeField] private Slider gaugeSlider;
    
    [Header("Gauge Settings")]
    [SerializeField] private float maxGauge = 200f;
    [SerializeField] private float currentGauge = 0f;
    

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