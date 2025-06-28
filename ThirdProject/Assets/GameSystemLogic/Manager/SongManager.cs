using UnityEngine;
using System.Collections;

public class SongManager : MonoBehaviour
{
    public static SongManager Instance { get; private set; }

    [Header("Audio Settings")]
    [SerializeField] private AudioSource musicAudioSource;

    private Coroutine currentSongCoroutine;
    private AudioClip originalClip;
    private float originalVolume;

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
        if (musicAudioSource != null)
        {
            originalClip = musicAudioSource.clip;
            originalVolume = musicAudioSource.volume;
        }
    }

    public void PlaySong(SongQuestionData songData)
    {
        if (musicAudioSource == null)
        {
            Debug.LogWarning("AudioSource가 없습니다.");
            return;
        }

        AudioClip songToPlay = GetRandomSong(songData);
        
        if (songToPlay == null)
        {
            Debug.LogWarning("재생할 노래가 없습니다.");
            return;
        }

        StopCurrentSong();

        currentSongCoroutine = StartCoroutine(PlaySongCoroutine(songData, songToPlay));
    }

    private AudioClip GetRandomSong(SongQuestionData songData)
    {
        if (songData.songClips != null && songData.songClips.Length > 0)
        {
            return GetRandomSongFromClipData(songData.songClips);
        }
        
        return null;
    }

    private AudioClip GetRandomSongFromClipData(SongClipData[] songClips)
    {
        for (int i = 0; i < songClips.Length; i++)
        {
            if (songClips[i].clip != null && Random.value <= songClips[i].playChance)
            {
                return songClips[i].clip;
            }
        }
        
        var availableClips = new System.Collections.Generic.List<AudioClip>();
        foreach (var clipData in songClips)
        {
            if (clipData.clip != null)
            {
                availableClips.Add(clipData.clip);
            }
        }
        
        if (availableClips.Count > 0)
        {
            int fallbackIndex = Random.Range(0, availableClips.Count);
            return availableClips[fallbackIndex];
        }
        
        return null;
    }

    private IEnumerator PlaySongCoroutine(SongQuestionData songData, AudioClip selectedSong)
    {
        musicAudioSource.clip = selectedSong;
        musicAudioSource.volume = songData.volume;
        musicAudioSource.loop = songData.loop;
        
        musicAudioSource.Play();
        
        Debug.Log($"노래 재생 시작: {selectedSong.name} ({selectedSong.length:F1}초)");

        if (!songData.loop)
        {
            while (musicAudioSource.isPlaying)
            {
                yield return null;
            }
            
            StopSong();
        }
    }

    public void StopCurrentSong()
    {
        if (currentSongCoroutine != null)
        {
            StopCoroutine(currentSongCoroutine);
            currentSongCoroutine = null;
        }
        
        StopSong();
    }

    private void StopSong()
    {
        if (musicAudioSource != null)
        {
            musicAudioSource.Stop();
            
            if (originalClip != null)
            {
                musicAudioSource.clip = originalClip;
                musicAudioSource.volume = originalVolume;
                musicAudioSource.loop = true;
                musicAudioSource.Play();
            }
        }
        
        Debug.Log("노래 재생 종료");
    }

    public bool IsSongPlaying()
    {
        return musicAudioSource != null && musicAudioSource.isPlaying && currentSongCoroutine != null;
    }

    private void OnDestroy()
    {
        StopCurrentSong();
    }
}