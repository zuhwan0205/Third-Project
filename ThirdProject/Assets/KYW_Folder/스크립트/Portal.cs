using System.Collections.Generic;
using UnityEngine;

// 플레이어가 닿으면 지정된 위치로 이동시키는 포탈 스크립트
public class Portal : MonoBehaviour
{
    [SerializeField] private Transform targetPosition; // 이동할 위치
    [SerializeField] private float portalCooldown = 0.5f;

    // 플레이어별 마지막 이동 시간 기록
    private Dictionary<Transform, float> lastTeleportTime = new();

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && targetPosition != null)
        {
            float lastTime = -999f;
            lastTeleportTime.TryGetValue(other.transform, out lastTime);

            if (Time.time - lastTime < portalCooldown)
                return; // 쿨타임 내면 무시

            // 이동
            var cc = other.GetComponent<CharacterController>();
            if (cc != null)
            {
                cc.enabled = false;
                other.transform.position = targetPosition.position;
                cc.enabled = true;
            }
            else
            {
                other.transform.position = targetPosition.position;
            }

            lastTeleportTime[other.transform] = Time.time;
        }
    }
} 