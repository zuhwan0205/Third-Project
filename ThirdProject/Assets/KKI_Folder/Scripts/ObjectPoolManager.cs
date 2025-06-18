using System.Collections.Generic;
using UnityEngine;

public enum PoolKey
{
    Bullet,
    ShotgunPellet,
    Arrow,
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
        {
            var pool = new Queue<GameObject>();
            pools[setting.key] = pool;
            poolSettingsDict[setting.key] = setting;
            activeObjects[setting.key] = new HashSet<GameObject>();
            
            for (int i = 0; i < setting.initialSize; i ++) // 처음 생성 크기만큼 생성하기.
            {
                GameObject obj = Instantiate(setting.prefab, transform); // 자식으로 생성 
                obj.SetActive(false); 
                pool.Enqueue(obj);
            }
        }
    }

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
                Debug.LogWarning($"Pool({key}) : 비어있고, 더 이상 확장이 되지 않음!");
                return false;
            }
        }

        obj.SetActive(true);
        activeObjects[key].Add(obj);
        component = obj.GetComponent<T>();

        return true;
    }

    
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