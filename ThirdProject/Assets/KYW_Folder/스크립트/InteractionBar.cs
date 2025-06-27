using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InteractionBar : MonoBehaviour
{
    public static InteractionBar Instance { get; private set; }

    [Header("상호작용 UI")]
    [SerializeField] Slider interactionSlider;

    private Coroutine currentInteractionCoroutine;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        // 시작시 상호작용 바 숨기기
        HideInteractionBar();
    }

    // 상호작용 바 시작
    public void StartInteractionBar(float duration)
    {
        // 이미 진행 중인 상호작용이 있으면 중단
        if (currentInteractionCoroutine != null)
        {
            StopCoroutine(currentInteractionCoroutine);
        }

        // 상호작용 바 표시
        ShowInteractionBar();
        
        // 상호작용 코루틴 시작
        currentInteractionCoroutine = StartCoroutine(InteractionBarCoroutine(duration));
    }

    // 상호작용 바 코루틴
    private IEnumerator InteractionBarCoroutine(float duration)
    {
        float elapsedTime = 0f;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / duration;
            
            // 슬라이더 값 업데이트
            if (interactionSlider != null)
            {
                interactionSlider.value = progress;
            }
            
            yield return null;
        }
        
        // 상호작용 완료
        if (interactionSlider != null)
        {
            interactionSlider.value = 1f;
        }
        
        // 상호작용 바 숨기기
        HideInteractionBar();
        
        currentInteractionCoroutine = null;
    }

    // 상호작용 바 표시
    private void ShowInteractionBar()
    {
        if (interactionSlider != null)
        {
            interactionSlider.gameObject.SetActive(true);
            interactionSlider.value = 0f;
        }
    }

    // 상호작용 바 숨기기
    private void HideInteractionBar()
    {
        if (interactionSlider != null)
        {
            interactionSlider.gameObject.SetActive(false);
            interactionSlider.value = 0f;
        }
    }

    // 상호작용 중인지 확인
    public bool IsInteracting()
    {
        return currentInteractionCoroutine != null;
    }
} 