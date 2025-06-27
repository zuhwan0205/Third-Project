using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameModeManager.GameMode CurrentGameMode => GameModeManager.CurrentMode;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    private void Start()
    {
        if (CurrentGameMode == GameModeManager.GameMode.Hard)
        {
            Debug.Log("[GameManager] Mode is Hard");
        }
        else
        {
            Debug.Log("[GameManager] 쫄보ㅋ");
        }
        SpawnManager.Instance.SpawnPlayer();
    }
    
}
