using UnityEngine;

public class UseItemSpawn : MonoBehaviour
{
    [SerializeField] private GameObject fKeyPotionPrefab;
    [SerializeField] private GameObject gKeyCanPrefab;
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            // 포션이 1개 이상 있을 때만 실행
            if (InventoryManager.Instance.CheckItemCount("포션") > 0)
            {
                SoundEffectManager.Instance.PlaySound("먹기");
                SpawnPrefab(fKeyPotionPrefab);
                InventoryManager.Instance.RemoveItem("포션");
                PlayerController.Instance.Heal(25);
            }
            else
            {
                Debug.Log("포션이 부족합니다!");
            }
        }
        
        if (Input.GetKeyDown(KeyCode.G))
        {
            // 통조림이 1개 이상 있을 때만 실행
            if (InventoryManager.Instance.CheckItemCount("통조림") > 0)
            {
                SoundEffectManager.Instance.PlaySound("먹기");
                SpawnPrefab(gKeyCanPrefab);
                InventoryManager.Instance.RemoveItem("통조림");
                PlayerController.Instance.IncreaseHunger(25);
            }
            else
            {
                Debug.Log("통조림이 부족합니다!");
            }
        }
    }
    
    private void SpawnPrefab(GameObject prefab)
    {
        if (prefab == null) return;
        
        GameObject spawnedObject = Instantiate(prefab, transform.position, transform.rotation);
        
        // 콜라이더 삭제
        Collider[] colliders = spawnedObject.GetComponentsInChildren<Collider>();
        foreach (Collider collider in colliders)
        {
            Destroy(collider);
        }
    }
}
