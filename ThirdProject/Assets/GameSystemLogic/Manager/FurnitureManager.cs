using UnityEngine;
using System.Collections.Generic;

public class FurnitureManager : MonoBehaviour
{
    public static FurnitureManager Instance { get; private set; }
    
    private Dictionary<string, GameObject> furnitureDict = new Dictionary<string, GameObject>();
    private HashSet<string> spawnedFurniture = new HashSet<string>();
    private HashSet<string> destroyedFurniture = new HashSet<string>();
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeFurnitureDict();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void InitializeFurnitureDict()
    {
        furnitureDict.Clear();
        
        // 씬의 모든 GameObject 검사 (비활성화된 것도 포함)
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        
        foreach (GameObject obj in allObjects)
        {
            // 씬에 있는 오브젝트만 (프리팹이나 에셋 제외)
            if (obj.scene.IsValid() && obj.CompareTag("Furniture"))
            {
                string furnitureID = obj.name;
                furnitureDict[furnitureID] = obj;
                
                // 처음에는 모든 가구를 비활성화
                obj.SetActive(false);
                
                Debug.Log($"가구 등록: ID='{furnitureID}', Object='{obj.name}'");
            }
        }
        
        Debug.Log($"총 {furnitureDict.Count}개의 가구가 등록되었습니다.");
    }
    
    public void SpawnFurniture(string furnitureID)
    {
        if (furnitureDict.ContainsKey(furnitureID))
        {
            GameObject furniture = furnitureDict[furnitureID];
            furniture.SetActive(true);
            spawnedFurniture.Add(furnitureID);
            
            if (destroyedFurniture.Contains(furnitureID))
            {
                destroyedFurniture.Remove(furnitureID);
            }
            
        }
        else
        {
            Debug.LogWarning($"가구 ID '{furnitureID}'를 찾을 수 없습니다.");
        }
    }
    
    public void DestroyFurniture(string furnitureID)
    {
        if (spawnedFurniture.Contains(furnitureID))
        {
            if (furnitureDict.ContainsKey(furnitureID))
            {
                GameObject furniture = furnitureDict[furnitureID];
                furniture.SetActive(false);
            }
            
            spawnedFurniture.Remove(furnitureID);
            destroyedFurniture.Add(furnitureID);
            
            Debug.Log($"가구 '{furnitureID}' 파괴됨");
        }
    }
    
    public bool IsFurnitureSpawned(string furnitureID)
    {
        return spawnedFurniture.Contains(furnitureID);
    }
    
    public bool IsFurnitureDestroyed(string furnitureID)
    {
        return destroyedFurniture.Contains(furnitureID);
    }
    
    public bool CanShowFurnitureQuestion(string furnitureID)
    {
        // 스폰되지 않았거나, 파괴된 경우에만 질문 가능
        return !spawnedFurniture.Contains(furnitureID) || destroyedFurniture.Contains(furnitureID);
    }
}