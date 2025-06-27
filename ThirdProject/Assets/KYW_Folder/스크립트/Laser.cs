using UnityEngine;

// 플레이어가 닿으면 체력을 깎는 레이저 트리거
public class Laser : MonoBehaviour
{
    [SerializeField] private float damage = 10f;

    private void OnTriggerEnter(Collider other)
    {
        // 플레이어 태그로 충돌 체크
        if (other.CompareTag("Player"))
        {
            // 체력 감소

            // 체력 fill 업데이트
        }
    }
} 