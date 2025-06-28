using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private GameObject lazerObject;

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
        InitializeHardMode();
        SpawnManager.Instance.SpawnPlayer();
    }
    
    private void InitializeHardMode()
    {
        if (CurrentGameMode == GameModeManager.GameMode.Hard)
        {
            EnableHardModeFeatures();
        }
        else
        {
            DisableHardModeFeatures();
        }
    }
    
    private void EnableHardModeFeatures()
    {
        if (lazerObject == null)
        {
            lazerObject = FindInactiveObjectWithTag("Lazer");
        }
        
        if (lazerObject != null)
        {
            lazerObject.SetActive(true);
            Debug.Log("[GameManager] Lazer 활성화됨");
        }
        else
        {
            Debug.LogWarning("[GameManager] 'Lazer' 태그를 가진 오브젝트를 찾을 수 없습니다!");
        }
    }
    
    private void DisableHardModeFeatures()
    {
        if (lazerObject == null)
        {
            lazerObject = FindInactiveObjectWithTag("Lazer");
        }
        
        if (lazerObject != null)
        {
            lazerObject.SetActive(false);
        }
    }
    
    private GameObject FindInactiveObjectWithTag(string tag)
    {
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        
        foreach (GameObject obj in allObjects)
        {
            if (obj.scene.IsValid() && obj.CompareTag(tag))
            {
                return obj;
            }
        }
        
        return null;
    }
}