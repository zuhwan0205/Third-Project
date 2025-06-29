using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class EnvironmentManager : MonoBehaviour
{
    public static EnvironmentManager Instance { get; private set; }

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    
    [Header("Ending Effects")]
    [SerializeField] private Image blackoutOverlay; // UI Canvas에 있는 검은색 이미지
    
    private bool originalFogEnabled;
    private Color originalFogColor;
    private float originalFogDensity;
    private float originalAmbientIntensity;
    private float originalReflectionIntensity;

    private Coroutine currentEffectCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        SaveOriginalEnvironmentSettings();
        
        // 블랙아웃 오버레이 초기화
        if (blackoutOverlay != null)
        {
            Color transparent = blackoutOverlay.color;
            transparent.a = 0f;
            blackoutOverlay.color = transparent;
            blackoutOverlay.gameObject.SetActive(false);
        }
    }

    public void ApplyEnvironmentEffect(EnvironmentQuestionData environmentData)
    {
        if (currentEffectCoroutine != null)
        {
            StopCoroutine(currentEffectCoroutine);
            ResetEnvironment();
        }
        
        currentEffectCoroutine = StartCoroutine(EnvironmentEffectCoroutine(environmentData));
    }

    private IEnumerator EnvironmentEffectCoroutine(EnvironmentQuestionData data)
    {
        if (data.dimSkybox)
        {
            ApplySkyboxDimming(data.skyIntensityMultiplier);
        }

        if (data.enableFog)
        {
            ApplyFogEffect(data.fogColor, data.fogDensity);
        }

        if (data.playSound && data.environmentSound != null && audioSource != null)
        {
            audioSource.clip = data.environmentSound;
            audioSource.Play();
        }
        yield return new WaitForSeconds(data.effectDuration);
        
        ResetEnvironment();
        
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
        currentEffectCoroutine = null;
    }

    public void StartEndingSequence()
    {
        StartCoroutine(EndingSequenceCoroutine());
    }

    private IEnumerator EndingSequenceCoroutine()
    {
        float duration = 10f;
        
        // 수면 마취 효과 (얕은 회색 Fog)
        Color sleepFogColor = new Color(0.7f, 0.7f, 0.7f, 1f); // 얕은 회색
        RenderSettings.fog = true;
        RenderSettings.fogColor = sleepFogColor;
        
        // Fog 서서히 차오르게 하기
        DOTween.To(() => RenderSettings.fogDensity, x => RenderSettings.fogDensity = x, 0.1f, duration)
            .SetEase(Ease.InQuad);
        
        // 동시에 블랙아웃 효과
        if (blackoutOverlay != null)
        {
            blackoutOverlay.gameObject.SetActive(true);
            blackoutOverlay.DOFade(1f, duration).SetEase(Ease.InQuad);
        }
        
        // 환경 어둡게 하기
        DOTween.To(() => RenderSettings.ambientIntensity, x => RenderSettings.ambientIntensity = x, 0f, duration)
            .SetEase(Ease.InQuad);
        
        DOTween.To(() => RenderSettings.reflectionIntensity, x => RenderSettings.reflectionIntensity = x, 0f, duration)
            .SetEase(Ease.InQuad);
        
        // 10초 대기
        yield return new WaitForSeconds(duration);
        
        // 엔딩 씬으로 이동
        SceneManager.LoadScene("EndingScene");
    }

    private void ApplySkyboxDimming(float intensityMultiplier)
    {
        RenderSettings.ambientIntensity = originalAmbientIntensity * intensityMultiplier;
        RenderSettings.reflectionIntensity = originalReflectionIntensity * (intensityMultiplier * 2f);
    }

    private void ApplyFogEffect(Color fogColor, float fogDensity)
    {
        RenderSettings.fog = true;
        RenderSettings.fogColor = fogColor;
        RenderSettings.fogDensity = fogDensity;
    }

    private void ResetEnvironment()
    {
        RenderSettings.ambientIntensity = originalAmbientIntensity;
        RenderSettings.reflectionIntensity = originalReflectionIntensity;
        
        RenderSettings.fog = originalFogEnabled;
        RenderSettings.fogColor = originalFogColor;
        RenderSettings.fogDensity = originalFogDensity;
    }

    private void SaveOriginalEnvironmentSettings()
    {
        originalFogEnabled = RenderSettings.fog;
        originalFogColor = RenderSettings.fogColor;
        originalFogDensity = RenderSettings.fogDensity;
        originalAmbientIntensity = RenderSettings.ambientIntensity;
        originalReflectionIntensity = RenderSettings.reflectionIntensity;
    }

    private void OnDestroy()
    {
        ResetEnvironment();
    }
}