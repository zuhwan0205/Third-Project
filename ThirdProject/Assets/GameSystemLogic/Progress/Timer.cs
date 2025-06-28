using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class Timer : MonoBehaviour
{
    public static Timer Instance { get; private set; }

    [Header("UI Components")]
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Timer Settings")]
    [SerializeField] private float defaultTimerDuration = 20f;
    
    private float currentTimer = 0f;
    private float timerDuration = 20f;
    private bool isTimerRunning = false;
    private bool hasAnswered = false;
    
    public static event Action OnTimeUp;

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
        InitializeTimer();
    }

    private void Update()
    {
        if (isTimerRunning)
        {
            UpdateTimerDisplay();
        }
    }

    private void InitializeTimer()
    {
        if (timerText != null)
            timerText.text = defaultTimerDuration.ToString();
    }

    public void StartTimer(float duration = 0f)
    {
        timerDuration = duration > 0 ? duration : defaultTimerDuration;
        currentTimer = timerDuration;
        isTimerRunning = true;
        hasAnswered = false;
    }
    
    public void StopTimer()
    {
        isTimerRunning = false;
    }
    
    public void SetAnswered()
    {
        hasAnswered = true;
        StopTimer();
    }
    
    private void UpdateTimerDisplay()
    {
        if (!isTimerRunning) return;
        currentTimer -= Time.deltaTime;
        int seconds = Mathf.CeilToInt(currentTimer);
        
        if (timerText != null)
        {
            timerText.text = seconds.ToString();
        }
        
        if (currentTimer <= 0f && !hasAnswered)
        {
            HandleTimeUp();
        }
    }
    
    private void HandleTimeUp()
    {
        hasAnswered = true;
        StopTimer();
        OnTimeUp?.Invoke();
    }
    
    public float GetRemainingTime()
    {
        return currentTimer;
    }
    
    public bool IsTimerRunning()
    {
        return isTimerRunning;
    }
    
    public void SetDefaultDuration(float duration)
    {
        defaultTimerDuration = duration;
    }
}