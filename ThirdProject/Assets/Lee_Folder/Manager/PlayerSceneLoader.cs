using UnityEngine;
using Fusion;
using System.Collections;

// LeeScene에 로드된 각 플레이어가 GameManager에게 로드 완료를 알리는 컴포넌트
public class PlayerSceneLoader : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(WaitForGameManagerAndNotify());
    }
    
    IEnumerator WaitForGameManagerAndNotify()
    {
        var runner = FindFirstObjectByType<NetworkRunner>();
        if (runner == null)
        {
            Debug.LogError("[PlayerSceneLoader] NetworkRunner not found");
            yield break;
        }
        
        while (GameManager.Instance == null || 
               GameManager.Instance.Object == null || 
               !GameManager.Instance.Object.IsValid)
        {
            yield return new WaitForSeconds(0.2f);
        }
        
        // 추가로 GameManager가 완전히 초기화될 때까지 대기
        yield return new WaitForSeconds(0.5f);
        
        GameManager.Instance.RPC_PlayerLoadedIntoScene(runner.LocalPlayer);
    }
}