using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class Bullet : Projectile
{
    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
    }
}