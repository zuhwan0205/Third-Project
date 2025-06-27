using UnityEngine;
using TMPro;
using DG.Tweening;

public class QuestionManager : MonoBehaviour
{
    public static QuestionManager Instance { get; private set; }

    [Header("Question Sources")]
    [SerializeField] private IntroTextBank introTextBank;
    [SerializeField] private NaturalQuestion naturalQuestion;
    [SerializeField] private RewardQuestion rewardQuestion;
    [SerializeField] private MonsterQuestion monsterQuestion;

    [Header("UI")]
    [SerializeField] private TextMeshPro textMeshPro3D;
    [SerializeField] private float typingSpeed = 0.05f;

    private Tween typingTween;
    private int introIndex = 0;

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
            Debug.Log("인트로 텍스트 종료");
            return;
        }

        string intro = introTextBank.startTexts[introIndex];
        introIndex++;

        if (typingTween != null && typingTween.IsActive())
            typingTween.Kill();

        typingTween = TypingText.Type(textMeshPro3D, intro, typingSpeed)
            .OnComplete(() =>
            {
                Debug.Log($"[{introIndex}] 다음 인트로로 진행");
                Invoke(nameof(ShowIntroText), 1.5f);
            });
    }

    public void OnPlayerAnswered(bool isYes)
    {
        Debug.Log($"플레이어 응답: {(isYes ? "예" : "아니오")}");
    }
}