using UnityEngine;
using System.Collections;

public class EnvironmentManager : MonoBehaviour
{
    public static EnvironmentManager Instance { get; private set; }

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    
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

    private void ApplySkyboxDimming(float intensityMultiplier)
    {
        RenderSettings.ambientIntensity = originalAmbientIntensity * intensityMultiplier;
        RenderSettings.reflectionIntensity = originalReflectionIntensity * (intensityMultiplier * 2f); // 리플렉션은 조금 덜 어둡게
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