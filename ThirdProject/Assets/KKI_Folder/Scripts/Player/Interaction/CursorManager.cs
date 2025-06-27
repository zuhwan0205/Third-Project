using UnityEngine;
using Fusion;

public class CursorManager : NetworkBehaviour
{
    public static CursorManager Instance { get; private set; }

    [SerializeField] private GameObject cursor;
    private Animator cursorAnimator;

    private void Awake()
    {
        // 싱글톤 패턴: Instance 중복 방지
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Cursor 오브젝트가 인스펙터에서 할당 안 됐으면 찾기
        if (cursor == null)
            cursor = GameObject.Find("Cursor");

        if (cursor != null)
            cursorAnimator = cursor.GetComponent<Animator>();
        else
            Debug.LogError("Cursor 오브젝트를 찾을 수 없습니다!");
    }

    public void SetZoom(bool zoom)
    {
        if (cursorAnimator != null)
            cursorAnimator.SetBool("bZoom", zoom);
    }
}
