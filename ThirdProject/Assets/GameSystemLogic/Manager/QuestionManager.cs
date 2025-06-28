using UnityEngine;
using TMPro;
using DG.Tweening;
using System.Linq;

public class QuestionManager : MonoBehaviour
{
    public static QuestionManager Instance { get; private set; }

    [Header("Question Sources")]
    [SerializeField] private IntroTextBank introTextBank;
    [SerializeField] private NaturalQuestion naturalQuestion;
    [SerializeField] private RewardQuestion rewardQuestion;
    [SerializeField] private MonsterQuestion monsterQuestion;
    [SerializeField] private ComplexQuestion complexQuestion;

    [Header("UI")]
    [SerializeField] private TextMeshPro textMeshPro3D;
    [SerializeField] private float typingSpeed = 0.05f;

    private Tween typingTween;
    private int introIndex = 0;
    private bool waitingForPlayerInput = false;
    private RoomQuestion currentQuestion;
    private ComplexQuestionData currentComplexQuestion;

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
        ShowIntroText();
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

        typingTween = TypingText.Type(textMeshPro3D, intro, typingSpeed)
            .OnComplete(() =>
            {
                if (introIndex < introTextBank.startTexts.Length)
                {
                    Invoke(nameof(ShowIntroText), 1.5f);
                }
                else
                {
                    waitingForPlayerInput = true;
                }
            });
    }

    public void OnPlayerAnswered(bool isYes)
    {
        Debug.Log($"플레이어 응답: {(isYes ? "예" : "아니오")}");
        
        if (waitingForPlayerInput && isYes)
        {
            waitingForPlayerInput = false;
            ShowRandomQuestion();
        }
        else if (currentQuestion != null || currentComplexQuestion != null)
        {
            ProcessAnswer(isYes);
            
            Invoke(nameof(ShowRandomQuestion), 2f);
        }
    }

    private void ShowRandomQuestion()
    {
        // ComplexQuestion도 포함하여 4가지 타입 중 랜덤 선택
        int randomType = Random.Range(0, 4);
        RoomQuestion selectedQuestion = null;
        ComplexQuestionData selectedComplexQuestion = null;

        // 이전 질문 초기화
        currentQuestion = null;
        currentComplexQuestion = null;

        switch (randomType)
        {
            case 0:
                if (naturalQuestion != null && naturalQuestion.naturalQuestions.Length > 0)
                {
                    int randomIndex = Random.Range(0, naturalQuestion.naturalQuestions.Length);
                    selectedQuestion = naturalQuestion.naturalQuestions[randomIndex];
                }
                break;
            case 1:
                if (rewardQuestion != null && rewardQuestion.rewardQuestions.Length > 0)
                {
                    int randomIndex = Random.Range(0, rewardQuestion.rewardQuestions.Length);
                    selectedQuestion = rewardQuestion.rewardQuestions[randomIndex];
                }
                break;
            case 2:
                if (monsterQuestion != null && monsterQuestion.monsterQuestions.Length > 0)
                {
                    int randomIndex = Random.Range(0, monsterQuestion.monsterQuestions.Length);
                    selectedQuestion = monsterQuestion.monsterQuestions[randomIndex];
                }
                break;
            case 3:
                if (complexQuestion != null && complexQuestion.complexQuestions.Length > 0)
                {
                    int randomIndex = Random.Range(0, complexQuestion.complexQuestions.Length);
                    selectedComplexQuestion = complexQuestion.complexQuestions[randomIndex];
                }
                break;
        }

        if (selectedQuestion != null)
        {
            DisplayQuestion(selectedQuestion);
        }
        else if (selectedComplexQuestion != null)
        {
            DisplayComplexQuestion(selectedComplexQuestion);
        }
        else
        {
            Debug.LogWarning("선택된 질문이 없습니다!");
        }
    }

    private void DisplayQuestion(RoomQuestion question)
    {
        currentQuestion = question;
        
        if (typingTween != null && typingTween.IsActive())
            typingTween.Kill();

        typingTween = TypingText.Type(textMeshPro3D, question.questionText, typingSpeed);
    }

    private void DisplayComplexQuestion(ComplexQuestionData complexQuestionData)
    {
        currentComplexQuestion = complexQuestionData;
        
        if (typingTween != null && typingTween.IsActive())
            typingTween.Kill();

        typingTween = TypingText.Type(textMeshPro3D, complexQuestionData.questionText, typingSpeed);
    }

    private void ProcessAnswer(bool isYes)
    {
        if (currentQuestion != null)
        {
            ProcessRoomQuestion(isYes);
        }
        else if (currentComplexQuestion != null)
        {
            ProcessComplexQuestion(isYes);
        }
    }

    private void ProcessRoomQuestion(bool isYes)
    {
        if (currentQuestion == null) return;

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
    }

    private void ProcessComplexQuestion(bool isYes)
    {
        if (currentComplexQuestion == null) return;

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
    }
}