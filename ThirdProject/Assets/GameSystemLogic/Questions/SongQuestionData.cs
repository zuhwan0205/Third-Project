using UnityEngine;

[System.Serializable]
public class SongQuestionData
{
    [TextArea]
    public string questionText;
    public SongClipData[] songClips;
    [Range(0f, 1f)]
    public float volume = 0.5f;
    public bool loop = false;
    public float yesGaugeChange = 0f;
    public float noGaugeChange = 0f;
}
