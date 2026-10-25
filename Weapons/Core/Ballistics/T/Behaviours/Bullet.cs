using UnityEngine;

namespace EggCentric.Ballistics
{
    public abstract class Bullet : Projectile
    {
        [SerializeField] protected AngleBasedHitmark spawner;
    }
}