using UnityEngine;

public class Playertest : MonoBehaviour
{
    public float moveSpeed = 5f;          // 이동 속도
    public float rotationSpeed = 720f;    // 회전 속도 (도/초)

    void Update()
    {
        // 입력 받기 (WASD 또는 방향키)
        float h = Input.GetAxis("Horizontal");  // A/D, ←/→
        float v = Input.GetAxis("Vertical");    // W/S, ↑/↓

        // 입력 방향 벡터
        Vector3 direction = new Vector3(h, 0f, v).normalized;

        // 방향 입력이 있을 때만 이동
        if (direction.magnitude > 0.1f)
        {
            // 이동
            transform.position += direction * moveSpeed * Time.deltaTime;

            // 회전 (입력 방향으로 회전)
            Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }
    }
}
