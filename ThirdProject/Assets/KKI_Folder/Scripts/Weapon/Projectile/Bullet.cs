using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class Bullet : Projectile
{
    protected override void OnCollisionEnter(Collision other)
    {
        base.OnCollisionEnter(other);
    }
}