using UnityEngine;

// 이 클래스는 사운드 효과를 관리하는 싱글톤 매니저입니다.
[RequireComponent(typeof(AudioSource))]
public class SoundEffectManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static SoundEffectManager Instance { get; private set; }

    // 효과음을 재생할 오디오 소스
    private AudioSource audioSource;

    // 인스펙터에서 할당할 효과음 클립 배열
    [SerializeField]
    private AudioClip[] soundEffectClips;

    void Awake()
    {
        // 싱글톤 패턴 구현
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // 씬이 변경되어도 파괴되지 않도록 설정

        // AudioSource 컴포넌트 가져오기
        audioSource = GetComponent<AudioSource>();
    }

    // 이름으로 사운드 재생
    public void PlaySound(string clipName)
    {
        // 배열에서 이름이 일치하는 오디오 클립 찾기
        foreach (var clip in soundEffectClips)
        {
            if (clip != null && clip.name == clipName)
            {
                // 찾은 클립을 PlayOneShot으로 재생 (중첩 재생 가능)
                audioSource.PlayOneShot(clip);
                return;
            }
        }

        // 클립을 찾지 못한 경우 경고 메시지 출력
        Debug.LogWarning($"SoundEffectManager: '{clipName}' 사운드를 찾을 수 없습니다.");
    }
} 