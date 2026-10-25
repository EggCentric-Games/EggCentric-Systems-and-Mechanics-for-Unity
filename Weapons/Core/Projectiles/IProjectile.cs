using EggCentric.Entities;
using EggCentric.LifeCycleHandling;

namespace EggCentric.Ballistics
{
    public interface IProjectile : IKinematicEntity, ITickable
    {
        public ProjectileConfig Config { get; }
    }
}