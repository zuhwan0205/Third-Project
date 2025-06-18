using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class Bullet : Projectile
{
    protected override void OnCollisionEnter(Collision other)
    {
        var target = other.collider.GetComponent<MonsterController>();
        if (target != null)
            // 데미지 처리
            // target.TakeDamage()
            Debug.Log("몬스터 데미지 처리!");
        else
            Debug.Log("몬스터 안 맞음..!");
    

        base.OnCollisionEnter(other);
    }
}