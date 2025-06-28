using UnityEngine;

[System.Serializable]
public class SongClipData
{
    public AudioClip clip;
    [Range(0f, 1f)]
    public float playChance = 1f;
}
