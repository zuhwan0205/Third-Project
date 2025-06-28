using UnityEngine;

public class RandamPrefabSpwaner : MonoBehaviour
{
    // 생성할 프리팹들을 담을 배열
    public GameObject[] prefabs;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 0.5초 후에 SpawnPrefab 메서드를 호출합니다.
        Invoke(nameof(SpawnPrefab), 0.5f);
    }

    void SpawnPrefab()
    {
        // 프리팹 배열이 비어있거나 내용이 없는지 확인합니다.
        if (prefabs == null || prefabs.Length == 0)
        {
            Debug.LogError("프리팹 배열이 비어있습니다.");
            return;
        }

        // 0부터 프리팹 배열의 길이 사이에서 랜덤 인덱스를 가져옵니다.
        int randomIndex = Random.Range(0, prefabs.Length);
        GameObject prefabToSpawn = prefabs[randomIndex];

        // 선택된 프리팹을 현재 게임 오브젝트의 위치와 회전값으로 생성합니다.
        Instantiate(prefabToSpawn, transform.position, transform.rotation);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
