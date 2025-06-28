using UnityEngine;

[System.Serializable]
public class EnvironmentQuestionData
{
    [TextArea]
    public string questionText;
    
    [Header("Sky Effect")]
    public bool dimSkybox;
    public float skyIntensityMultiplier = 0.05f;
    public float effectDuration = 30f;
    
    [Header("Additional Effects")]
    public bool enableFog;
    public Color fogColor = Color.gray;
    public float fogDensity = 0.1f;
    
    public bool playSound;
    public AudioClip environmentSound;
}