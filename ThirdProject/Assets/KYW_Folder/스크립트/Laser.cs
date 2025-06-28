using UnityEngine;

// 플레이어가 닿으면 체력을 깎는 레이저 트리거
public class Laser : MonoBehaviour
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private float speed = 2f; // 레이저 이동 속도
    [SerializeField] private float moveDistance = 12f; // X축 음의 방향으로 이동할 거리

    private Vector3 startPosition;
    private Vector3 endPosition;

    void Start()
    {
        // 초기 위치와 목표 위치 설정
        startPosition = transform.position;
        endPosition = startPosition + Vector3.left * moveDistance;
    }

    void Update()
    {
        // PingPong을 사용하여 0과 1 사이를 왕복하는 값을 만듭니다.
        float t = Mathf.PingPong(Time.time * speed, 1);
        // Lerp를 사용하여 시작 위치와 끝 위치 사이를 부드럽게 이동합니다.
        transform.position = Vector3.Lerp(startPosition, endPosition, t);
    }

    private void OnTriggerEnter(Collider other)
    {
        // 플레이어 태그로 충돌 체크
        if (other.CompareTag("Player"))
        {
            // 체력 감소
            PlayerController.Instance.TakeDamage(damage);
        }
    }
} 