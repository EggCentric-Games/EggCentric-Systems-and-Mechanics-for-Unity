using UnityEngine;

namespace EggCentric.Ballistics
{
    public abstract class Bullet : Projectile, IKinematic
    {
        [SerializeField] protected AngleBasedHitmark spawner;
    }
}