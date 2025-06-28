using UnityEngine;

// IInteractable 인터페이스를 구현하여 상호작용 가능한 라디오를 만듭니다.
[RequireComponent(typeof(AudioSource))]
public class Radio : MonoBehaviour, IInteractable
{
    private AudioSource audioSource;

    void Awake()
    {
        // 이 게임 오브젝트에 연결된 AudioSource 컴포넌트를 가져옵니다.
        audioSource = GetComponent<AudioSource>();
    }

    // 상호작용 메서드
    public void Interact()
    {
        // 오디오가 재생 중이면 일시정지하고, 멈춰있으면 재생합니다.
        if (audioSource.isPlaying)
        {
            audioSource.Pause();
        }
        else
        {
            audioSource.Play();
        }
    }

    // 상호작용 UI에 표시될 텍스트를 반환하는 메서드
    public string GetInteractText()
    {
        return audioSource.isPlaying ? "라디오 끄기" : "라디오 켜기";
    }
} 