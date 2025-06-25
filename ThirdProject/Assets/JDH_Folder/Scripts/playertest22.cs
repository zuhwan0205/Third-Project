using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHp = 100;
    private int currentHp;

    void Start()
    {
        currentHp = maxHp;
    }

    public void TakeDamage(int amount)
    {
        currentHp -= amount;
        Debug.Log($"플레이어 피해: {amount} / 남은 체력: {currentHp}");

        if (currentHp <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("플레이어 사망");
        // TODO: 게임 오버 처리
    }

    public int GetCurrentHealth()
    {
        return currentHp;
    }
}
