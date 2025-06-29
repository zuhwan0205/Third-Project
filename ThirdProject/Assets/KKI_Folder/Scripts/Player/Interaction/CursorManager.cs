using UnityEngine;
using UnityEngine.UI;

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance;
    [SerializeField] private Text interactionText;

    public Text InteractionText => interactionText;

    void Awake()
    {
        if (Instance != null && Instance == this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this);


        CursorHide();
    }

    void CursorHide()
    {
        Cursor.lockState = CursorLockMode.Locked; // 커서 중앙 고정
        Cursor.visible = false;                   // 커서 숨김
    }

    void CursorShow()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
