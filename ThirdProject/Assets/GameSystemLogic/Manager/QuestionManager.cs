using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;
using System.Collections.Generic;

public class QuestionManager : MonoBehaviour
{
    public static QuestionManager Instance { get; private set; }

    [Header("Question Sources")]
    [SerializeField] private IntroTextBank introTextBank;
    [SerializeField] private EndingTextBank endingTextBank; // 추가
    [SerializeField] private NaturalQuestion naturalQuestion;
    [SerializeField] private RewardQuestion rewardQuestion;
    [SerializeField] private MonsterQuestion monsterQuestion;
    [SerializeField] private ComplexQuestion complexQuestion;
    [SerializeField] private EnvironmentQuestion environmentQuestion;
    [SerializeField] private SongQuestion songQuestion;
    [SerializeField] private FurnitureQuestion furnitureQuestion;

    [Header("UI Components")]
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private TextMeshProUGUI yesScoreText;
    [SerializeField] private TextMeshProUGUI noScoreText;
    [SerializeField] private float typingSpeed = 0.05f;

    [Header("Duplicate Prevention")]
    [SerializeField] private float environmentQuestionCooldown = 60f;

    private Tween typingTween;
    private int introIndex = 0;
    private bool waitingForPlayerInput = false;
    private RoomQuestion currentQuestion;
    private ComplexQuestionData currentComplexQuestion;
    private EnvironmentQuestionData currentEnvironmentQuestion;
    private SongQuestionData currentSongQuestion;
    private FurnitureQuestionData currentFurnitureQuestion;
    
    private float lastEnvironmentQuestionTime = -999f;
    private HashSet<EnvironmentQuestionData> usedEnvironmentQuestions = new HashSet<EnvironmentQuestionData>();
    
    // 엔딩 관련 변수들
    private int endingTextIndex = 0;
    private bool isPlayingEnding = false;
    private EndingTextBank currentEndingTextBank;

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
        Timer.OnTimeUp += HandleTimeUp;
        
        InitializeUI();
        ShowIntroText();
    }

    private void OnDestroy()
    {
        Timer.OnTimeUp -= HandleTimeUp;
    }

    private void InitializeUI()
    {
        if (yesScoreText != null)
            yesScoreText.text = "";
        
        if (noScoreText != null)
            noScoreText.text = "";
    }

    private void ShowIntroText()
    {
        if (introTextBank == null || introTextBank.startTexts == null || introTextBank.startTexts.Length == 0)
        {
            Debug.LogWarning("IntroTextBank is empty.");
            return;
        }

        if (introIndex >= introTextBank.startTexts.Length)
        {
            return;
        }

        string intro = introTextBank.startTexts[introIndex];
        introIndex++;

        if (typingTween != null && typingTween.IsActive())
            typingTween.Kill();

        typingTween = TypingText.Type(questionText, intro, typingSpeed)
            .OnComplete(() =>
            {
                if (introIndex < introTextBank.startTexts.Length)
                {
                    Invoke(nameof(ShowIntroText), 1.5f);
                }
                else
                {
                    waitingForPlayerInput = true;
                    Timer.Instance.StartTimer();
                }
            });
    }

    public void OnPlayerAnswered(bool isYes)
    {
        // 엔딩 중에는 플레이어 입력 무시
        if (isPlayingEnding)
        {
            return;
        }
        
        Timer.Instance.SetAnswered();
        TypingText.HideScoreTexts(yesScoreText, noScoreText);
        
        Debug.Log($"플레이어 응답: {(isYes ? "예" : "아니오")}");
        
        if (waitingForPlayerInput && isYes)
        {
            waitingForPlayerInput = false;
            ShowRandomQuestion();
        }
        else if (currentQuestion != null || currentComplexQuestion != null || currentEnvironmentQuestion != null || currentSongQuestion != null || currentFurnitureQuestion != null)
        {
            bool processSuccessful = ProcessAnswer(isYes);
            
            if (processSuccessful)
            {
                Invoke(nameof(ShowRandomQuestion), 2f);
            }
        }
    }

    public void OnGaugeInsufficient(string message)
    {
        if (typingTween != null && typingTween.IsActive())
            typingTween.Kill();

        typingTween = TypingText.Type(questionText, message, typingSpeed)
            .OnComplete(() =>
            {
                Invoke(nameof(ReturnToCurrentQuestion), 2f);
            });
    }

    private void ReturnToCurrentQuestion()
    {
        if (currentQuestion != null)
        {
            DisplayQuestion(currentQuestion);
        }
        else if (currentComplexQuestion != null)
        {
            DisplayComplexQuestion(currentComplexQuestion);
        }
        else if (currentEnvironmentQuestion != null)
        {
            DisplayEnvironmentQuestion(currentEnvironmentQuestion);
        }
        else if (currentSongQuestion != null)
        {
            DisplaySongQuestion(currentSongQuestion);
        }
        else if (currentFurnitureQuestion != null)
        {
            DisplayFurnitureQuestion(currentFurnitureQuestion);
        }
    }

    private void ShowRandomQuestion()
    {
        bool canShowEnvironmentQuestion = CanShowEnvironmentQuestion();
        bool canShowSongQuestion = CanShowSongQuestion();
        bool canShowFurnitureQuestion = CanShowFurnitureQuestion();
        
        int availableTypes = 4;
        if (canShowEnvironmentQuestion) availableTypes++;
        if (canShowSongQuestion) availableTypes++;
        if (canShowFurnitureQuestion) availableTypes++;
        
        int randomType = Random.Range(0, availableTypes);
        
        RoomQuestion selectedQuestion = null;
        ComplexQuestionData selectedComplexQuestion = null;
        EnvironmentQuestionData selectedEnvironmentQuestion = null;
        SongQuestionData selectedSongQuestion = null;
        FurnitureQuestionData selectedFurnitureQuestion = null;
        
        currentQuestion = null;
        currentComplexQuestion = null;
        currentEnvironmentQuestion = null;
        currentSongQuestion = null;
        currentFurnitureQuestion = null;

        int typeIndex = 0;
        
        if (randomType == typeIndex++)
        {
            if (naturalQuestion != null && naturalQuestion.naturalQuestions.Length > 0)
            {
                int randomIndex = Random.Range(0, naturalQuestion.naturalQuestions.Length);
                selectedQuestion = naturalQuestion.naturalQuestions[randomIndex];
            }
        }
        else if (randomType == typeIndex++)
        {
            if (rewardQuestion != null && rewardQuestion.rewardQuestions.Length > 0)
            {
                int randomIndex = Random.Range(0, rewardQuestion.rewardQuestions.Length);
                selectedQuestion = rewardQuestion.rewardQuestions[randomIndex];
            }
        }
        else if (randomType == typeIndex++)
        {
            if (monsterQuestion != null && monsterQuestion.monsterQuestions.Length > 0)
            {
                int randomIndex = Random.Range(0, monsterQuestion.monsterQuestions.Length);
                selectedQuestion = monsterQuestion.monsterQuestions[randomIndex];
            }
        }
        else if (randomType == typeIndex++)
        {
            if (complexQuestion != null && complexQuestion.complexQuestions.Length > 0)
            {
                int randomIndex = Random.Range(0, complexQuestion.complexQuestions.Length);
                selectedComplexQuestion = complexQuestion.complexQuestions[randomIndex];
            }
        }
        else if (canShowEnvironmentQuestion && randomType == typeIndex++)
        {
            if (environmentQuestion != null && environmentQuestion.environmentQuestions.Length > 0)
            {
                selectedEnvironmentQuestion = GetAvailableEnvironmentQuestion();
            }
        }
        else if (canShowSongQuestion && randomType == typeIndex++)
        {
            if (songQuestion != null && songQuestion.songQuestions.Length > 0)
            {
                int randomIndex = Random.Range(0, songQuestion.songQuestions.Length);
                selectedSongQuestion = songQuestion.songQuestions[randomIndex];
            }
        }
        else if (canShowFurnitureQuestion && randomType == typeIndex++)
        {
            if (furnitureQuestion != null && furnitureQuestion.furnitureQuestions.Length > 0)
            {
                selectedFurnitureQuestion = GetAvailableFurnitureQuestion();
            }
        }

        if (selectedQuestion != null)
        {
            DisplayQuestion(selectedQuestion);
        }
        else if (selectedComplexQuestion != null)
        {
            DisplayComplexQuestion(selectedComplexQuestion);
        }
        else if (selectedEnvironmentQuestion != null)
        {
            DisplayEnvironmentQuestion(selectedEnvironmentQuestion);
        }
        else if (selectedSongQuestion != null)
        {
            DisplaySongQuestion(selectedSongQuestion);
        }
        else if (selectedFurnitureQuestion != null)
        {
            DisplayFurnitureQuestion(selectedFurnitureQuestion);
        }
        else
        {
            Debug.LogWarning("선택된 질문이 없습니다!");
        }
    }

    private bool CanShowEnvironmentQuestion()
    {
        float timeSinceLastEnvironmentQuestion = Time.time - lastEnvironmentQuestionTime;
        return timeSinceLastEnvironmentQuestion >= environmentQuestionCooldown;
    }

    private bool CanShowSongQuestion()
    {
        return SongManager.Instance == null || !SongManager.Instance.IsSongPlaying();
    }
    
    private bool CanShowFurnitureQuestion()
    {
        if (FurnitureManager.Instance == null || furnitureQuestion == null || furnitureQuestion.furnitureQuestions.Length == 0)
            return false;
        
        foreach (var furniture in furnitureQuestion.furnitureQuestions)
        {
            if (FurnitureManager.Instance.CanShowFurnitureQuestion(furniture.furnitureID))
            {
                return true;
            }
        }
        return false;
    }
    
    private FurnitureQuestionData GetAvailableFurnitureQuestion()
    {
        var availableQuestions = furnitureQuestion.furnitureQuestions
            .Where(q => FurnitureManager.Instance.CanShowFurnitureQuestion(q.furnitureID))
            .ToArray();

        if (availableQuestions.Length > 0)
        {
            int randomIndex = Random.Range(0, availableQuestions.Length);
            return availableQuestions[randomIndex];
        }

        return null;
    }

    private EnvironmentQuestionData GetAvailableEnvironmentQuestion()
    {
        var availableQuestions = environmentQuestion.environmentQuestions
            .Where(q => !usedEnvironmentQuestions.Contains(q))
            .ToArray();
        
        if (availableQuestions.Length == 0)
        {
            usedEnvironmentQuestions.Clear();
            availableQuestions = environmentQuestion.environmentQuestions;
            Debug.Log("모든 환경 질문을 사용했습니다. 질문 목록을 리셋합니다.");
        }

        if (availableQuestions.Length > 0)
        {
            int randomIndex = Random.Range(0, availableQuestions.Length);
            var selectedQuestion = availableQuestions[randomIndex];
            
            usedEnvironmentQuestions.Add(selectedQuestion);
            return selectedQuestion;
        }

        return null;
    }

    private void DisplayQuestion(RoomQuestion question)
    {
        currentQuestion = question;
        
        if (typingTween != null && typingTween.IsActive())
            typingTween.Kill();

        typingTween = TypingText.Type(questionText, question.questionText, typingSpeed)
            .OnComplete(() =>
            {
                TypingText.UpdateScoreTexts(yesScoreText, noScoreText, question.yesGaugeChange, question.noGaugeChange);
                Timer.Instance.StartTimer();
            });
    }

    private void DisplayComplexQuestion(ComplexQuestionData complexQuestionData)
    {
        currentComplexQuestion = complexQuestionData;
        
        if (typingTween != null && typingTween.IsActive())
            typingTween.Kill();

        typingTween = TypingText.Type(questionText, complexQuestionData.questionText, typingSpeed)
            .OnComplete(() =>
            {
                TypingText.UpdateScoreTexts(yesScoreText, noScoreText, complexQuestionData.yesGaugeChange, complexQuestionData.noGaugeChange);
                Timer.Instance.StartTimer();
            });
    }

    private void DisplayEnvironmentQuestion(EnvironmentQuestionData environmentQuestionData)
    {
        currentEnvironmentQuestion = environmentQuestionData;
        lastEnvironmentQuestionTime = Time.time;
        
        if (typingTween != null && typingTween.IsActive())
            typingTween.Kill();

        typingTween = TypingText.Type(questionText, environmentQuestionData.questionText, typingSpeed)
            .OnComplete(() =>
            {
                TypingText.UpdateScoreTexts(yesScoreText, noScoreText, environmentQuestionData.yesGaugeChange, environmentQuestionData.noGaugeChange);
                Timer.Instance.StartTimer();
            });
    }

    private void DisplaySongQuestion(SongQuestionData songQuestionData)
    {
        currentSongQuestion = songQuestionData;
        
        if (typingTween != null && typingTween.IsActive())
            typingTween.Kill();

        typingTween = TypingText.Type(questionText, songQuestionData.questionText, typingSpeed)
            .OnComplete(() =>
            {
                TypingText.UpdateScoreTexts(yesScoreText, noScoreText, songQuestionData.yesGaugeChange, songQuestionData.noGaugeChange);
                Timer.Instance.StartTimer();
            });
    }
    
    private void DisplayFurnitureQuestion(FurnitureQuestionData furnitureQuestionData)
    {
        currentFurnitureQuestion = furnitureQuestionData;
        
        if (typingTween != null && typingTween.IsActive())
            typingTween.Kill();

        typingTween = TypingText.Type(questionText, furnitureQuestionData.questionText, typingSpeed)
            .OnComplete(() =>
            {
                TypingText.UpdateScoreTexts(yesScoreText, noScoreText, furnitureQuestionData.yesGaugeChange, furnitureQuestionData.noGaugeChange);
                Timer.Instance.StartTimer();
            });
    }

    private bool ProcessAnswer(bool isYes)
    {
        if (currentQuestion != null)
        {
            return ProcessRoomQuestion(isYes);
        }
        else if (currentComplexQuestion != null)
        {
            return ProcessComplexQuestion(isYes);
        }
        else if (currentEnvironmentQuestion != null)
        {
            return ProcessEnvironmentQuestion(isYes);
        }
        else if (currentSongQuestion != null)
        {
            return ProcessSongQuestion(isYes);
        }
        else if (currentFurnitureQuestion != null)
        {
            return ProcessFurnitureQuestion(isYes);
        }
        return true;
    }

    private bool ProcessRoomQuestion(bool isYes)
    {
        if (currentQuestion == null) return true;
        
        float gaugeChange = isYes ? currentQuestion.yesGaugeChange : currentQuestion.noGaugeChange;
        if (Gauge.Instance != null)
        {
            bool canProceed = Gauge.Instance.TryAddGauge(gaugeChange);
            if (!canProceed)
            {
                return false;
            }
        }

        if (isYes)
        {
            if (currentQuestion.monsterList != null && currentQuestion.monsterList.Count > 0)
            {
                SpawnManager.Instance.SpawnMonsters(currentQuestion.monsterList);
            }
            
            if (currentQuestion.positiveRewards != null && currentQuestion.positiveRewards.Count > 0)
            {
                SpawnManager.Instance.SpawnItems(currentQuestion.positiveRewards);
            }
        }
        else
        {
            Debug.Log("플레이어가 거부했습니다. 아무것도 스폰하지 않습니다.");
        }
        return true;
    }

    private bool ProcessComplexQuestion(bool isYes)
    {
        if (currentComplexQuestion == null) return true;

        float gaugeChange = isYes ? currentComplexQuestion.yesGaugeChange : currentComplexQuestion.noGaugeChange;
        if (Gauge.Instance != null)
        {
            bool canProceed = Gauge.Instance.TryAddGauge(gaugeChange);
            if (!canProceed)
            {
                return false;
            }
        }

        if (isYes)
        {
            if (currentComplexQuestion.spawnMonster && currentComplexQuestion.extraMonsters != null)
            {
                SpawnManager.Instance.SpawnMonsters(currentComplexQuestion.extraMonsters.ToList());
            }
            
            if (currentComplexQuestion.spawnReward && currentComplexQuestion.extraRewards != null)
            {
                SpawnManager.Instance.SpawnItems(currentComplexQuestion.extraRewards.ToList());
            }
        }
        else
        {
            Debug.Log("플레이어가 복합 질문을 거부했습니다.");
        }
        return true;
    }

    private bool ProcessEnvironmentQuestion(bool isYes)
    {
        if (currentEnvironmentQuestion == null) return true;
        
        float gaugeChange = isYes ? currentEnvironmentQuestion.yesGaugeChange : currentEnvironmentQuestion.noGaugeChange;
        if (Gauge.Instance != null)
        {
            bool canProceed = Gauge.Instance.TryAddGauge(gaugeChange);
            if (!canProceed)
            {
                return false;
            }
        }

        if (isYes)
        {
            EnvironmentManager.Instance.ApplyEnvironmentEffect(currentEnvironmentQuestion);
        }
        else
        {
            Debug.Log("플레이어가 스카이박스 환경 효과 질문을 거부했습니다.");
        }
        return true;
    }

    private bool ProcessSongQuestion(bool isYes)
    {
        if (currentSongQuestion == null) return true;
        
        float gaugeChange = isYes ? currentSongQuestion.yesGaugeChange : currentSongQuestion.noGaugeChange;
        if (Gauge.Instance != null)
        {
            bool canProceed = Gauge.Instance.TryAddGauge(gaugeChange);
            if (!canProceed)
            {
                return false;
            }
        }

        if (isYes)
        {
            if (SongManager.Instance != null)
            {
                SongManager.Instance.PlaySong(currentSongQuestion);
            }
        }
        else
        {
            Debug.Log("플레이어가 노래 재생을 거부했습니다.");
        }
        return true;
    }
    
    private bool ProcessFurnitureQuestion(bool isYes)
    {
        if (currentFurnitureQuestion == null) return true;
        
        float gaugeChange = isYes ? currentFurnitureQuestion.yesGaugeChange : currentFurnitureQuestion.noGaugeChange;
        if (Gauge.Instance != null)
        {
            bool canProceed = Gauge.Instance.TryAddGauge(gaugeChange);
            if (!canProceed)
            {
                return false;
            }
        }

        if (isYes)
        {
            if (FurnitureManager.Instance != null)
            {
                FurnitureManager.Instance.SpawnFurniture(currentFurnitureQuestion.furnitureID);
            }
        }
        else
        {
            Debug.Log("플레이어가 가구 소환을 거부했습니다.");
        }
        return true;
    }
    
    private void HandleTimeUp()
    {
        if (isPlayingEnding)
        {
            return;
        }
        
        TypingText.HideScoreTexts(yesScoreText, noScoreText);
    
        if (waitingForPlayerInput)
        {
            waitingForPlayerInput = false;
            if (PlayerController.Instance != null)
            {
                PlayerController.Instance.TakeDamage(10);
            }
    
            if (Gauge.Instance != null)
            {
                Gauge.Instance.ForceReduceGauge(10);
            }
    
            Invoke(nameof(ShowRandomQuestion), 2f);
        }
        else if (currentQuestion != null || currentComplexQuestion != null || currentEnvironmentQuestion != null || currentSongQuestion != null || currentFurnitureQuestion != null)
        {
            if (PlayerController.Instance != null)
            {
                PlayerController.Instance.TakeDamage(10);
            }
    
            if (Gauge.Instance != null)
            {
                Gauge.Instance.ForceReduceGauge(10);
            }
            Invoke(nameof(ShowRandomQuestion), 2f);
        }
    }
    
    public void StartEndingSequence(EndingTextBank endingTextBank)
    {
        if (endingTextBank == null || endingTextBank.goodEndingTexts == null || endingTextBank.goodEndingTexts.Length == 0)
        {
            Debug.LogWarning("EndingTextBank가 비어있습니다. 바로 엔딩 환경 효과를 시작합니다.");
            if (Gauge.Instance != null)
            {
                Gauge.Instance.StartEndingEnvironmentEffect();
            }
            return;
            
        }
    
        isPlayingEnding = true;
        endingTextIndex = 0;
        currentEndingTextBank = endingTextBank;
        currentQuestion = null;
        currentComplexQuestion = null;
        currentEnvironmentQuestion = null;
        currentSongQuestion = null;
        currentFurnitureQuestion = null;
        waitingForPlayerInput = false;
        
        TypingText.HideScoreTexts(yesScoreText, noScoreText);
        
        ShowEndingText();
    }
    
    private void ShowEndingText()
    {
        if (currentEndingTextBank == null || endingTextIndex >= currentEndingTextBank.goodEndingTexts.Length)
        {
            isPlayingEnding = false;
            if (Gauge.Instance != null)
            {
                Gauge.Instance.StartEndingEnvironmentEffect();
            }
            return;
        }

        string endingText = currentEndingTextBank.goodEndingTexts[endingTextIndex];
        endingTextIndex++;

        if (typingTween != null && typingTween.IsActive())
            typingTween.Kill();

        typingTween = TypingText.Type(questionText, endingText, typingSpeed)
            .OnComplete(() =>
            {
                if (endingTextIndex < currentEndingTextBank.goodEndingTexts.Length)
                {
                    Invoke(nameof(ShowEndingText), 2f);
                }
                else
                {
                    isPlayingEnding = false;
                    Invoke(nameof(StartEndingEnvironmentEffect), 2f);
                }
            });
    }
    
    private void StartEndingEnvironmentEffect()
    {
        if (Gauge.Instance != null)
        {
            Gauge.Instance.StartEndingEnvironmentEffect();
        }
    }
    

    public void CancelAllInvokes()
    {
        CancelInvoke();
        if (typingTween != null && typingTween.IsActive())
        {
            typingTween.Kill();
        }
    }

    public void SetEndingMode(bool isEnding)
    {
        isPlayingEnding = isEnding;
    
        if (isEnding)
        {
            currentQuestion = null;
            currentComplexQuestion = null;
            currentEnvironmentQuestion = null;
            currentSongQuestion = null;
            currentFurnitureQuestion = null;
            
            waitingForPlayerInput = false;
        
            Debug.Log("엔딩 모드 활성화");
        }
    }
}