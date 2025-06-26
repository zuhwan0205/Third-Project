using System.Collections.Generic;
using UnityEngine;

public enum PoolKey
{
    Bullet,
    ShotgunPellet,
    Arrow,
    Axe,
    ShortSword,
    Pistol,
    Shotgun,
    Bow,
    AxeArm,
    ShortSwordArm,
    PistolArm,
    ShotgunArm,
    BowArm,
    // 추가 아이템
}

[System.Serializable]
public class PoolSettings
{
    public PoolKey key;
    public GameObject prefab;
    public int initialSize = 10;
    public int maxSize = 30;
    public bool autoExpand = true;
}

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Instance { get; private set; }

    [SerializeField] private List<PoolSettings> poolSettingsList;
    private Dictionary<PoolKey, Queue<GameObject>> pools = new();
    private Dictionary<PoolKey, PoolSettings> poolSettingsDict = new();
    private Dictionary<PoolKey, HashSet<GameObject>> activeObjects = new();

    public void Awake()
    {
        if (Instance != null && Instance != this) // 인스턴스 중복 방지
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject); // 씬 전환에서도 유지

        InitPool();    
    }

    private void InitPool()
    {
        foreach(var setting in poolSettingsList)
            CreatePool(setting.key, setting.prefab, setting.initialSize, setting.maxSize, setting.autoExpand);
    }

    /// <summary>
    /// 새로운 풀 동적 등록 (런타임 확장성)
    /// </summary>
    public void CreatePool(PoolKey key, GameObject prefab, int initialSize = 5, int maxSize = 30, bool autoExpand = true)
    {
        if (pools.ContainsKey(key)) return; // 이미 생성된 경우 무시

        var setting = new PoolSettings { key = key, prefab = prefab, initialSize = initialSize, maxSize = maxSize, autoExpand = autoExpand };
        poolSettingsDict[key] = setting;

        var pool = new Queue<GameObject>();
        pools[key] = pool;
        activeObjects[key] = new HashSet<GameObject>();

        for (int i = 0; i < initialSize; i++)
        {
            var obj = Instantiate(prefab, transform);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    /// <summary>
    /// 풀에서 오브젝트 가져오기 (타입별 반환)
    /// </summary>
    public bool TryGetObject<T>(PoolKey key, out T component) where T : Component
    {
        component = null;
        if (!pools.ContainsKey(key))
            return false;

        GameObject obj = null;
        if (pools[key].Count > 0)
        {
            obj = pools[key].Dequeue();
        }
        else 
        {
            var setting = poolSettingsDict[key];
            int totalCount = pools[key].Count + activeObjects[key].Count;
            if (setting.autoExpand && totalCount < setting.maxSize)
            {
                obj = Instantiate(setting.prefab);
            }
            else
            {
                Debug.LogWarning($"Pool({key}) : 풀 소진 및 maxSize 도달");
                return false;
            }
        }

        obj.SetActive(true);
        activeObjects[key].Add(obj);
        component = obj.GetComponent<T>();

        if (component == null)
            Debug.LogWarning($"풀 오브젝트({key})에서 타입({typeof(T)})을 찾을 수 없음");

        return true;
    }
    

    /// <summary>
    /// 풀 반환 (비활성화 후 풀에 등록)
    /// </summary>
    public void ReturnObject(PoolKey key, GameObject obj)
    {
        if (!pools.ContainsKey(key))
        {
            Debug.LogWarning($"오브젝트 풀에 '{key}'에 해당하는 key가 없어 반환이 안됩니다.");
            Destroy(obj);
            return;
        }

        obj.SetActive(false);
        pools[key].Enqueue(obj);
        activeObjects[key].Remove(obj);
    }

    // 런타임 프리팹 반환 (NetworkRunner 등에서 참조시)
    public GameObject GetPrefab(PoolKey key)
    {
        if (!poolSettingsDict.TryGetValue(key, out var setting))
        {
            Debug.LogError($"프리팹 미등록: {key}");
            return null;
        }
        return setting.prefab;
    }


    #region 풀 전체 개수(활성+비활성)
    public int GetTotalCount(PoolKey key)
    {
        if (!pools.ContainsKey(key)) return 0;
        return pools[key].Count + activeObjects[key].Count;
    }

    public int GetActiveCount(PoolKey key)
    {
        if (!activeObjects.ContainsKey(key)) return 0;
        return activeObjects[key].Count;
    }

    public int GetInactiveCount(PoolKey key)
    {
        if (!pools.ContainsKey(key)) return 0;
        return pools[key].Count;
    }
    #endregion


    # region 풀 초기화 함수 (씬 전환 때)
    public void ClearPool(PoolKey key)
    {
        if (!pools.ContainsKey(key)) return;

        foreach (var obj in pools[key])
        {
            Destroy(obj);
        }

        pools[key].Clear();
    }

    public void ClearAllPools()
    {
        foreach (var key in pools.Keys)
        {
            ClearPool(key);
        }
    }
    #endregion

}