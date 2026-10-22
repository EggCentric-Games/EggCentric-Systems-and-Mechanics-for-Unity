using EggCentric.LifeCycleHandling;

namespace EggCentric.Ballistics
{
    public interface IProjectile : IKinematic, ITickable
    {
        public ProjectileConfig Config { get; }
    }
}