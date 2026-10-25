using UnityEngine;

namespace EggCentric.Ballistics
{
    public struct ProjectileData
    {
        public ProjectileConfig Config { get; }
        public Vector3 Velocity { get; }

        public ProjectileData(ProjectileConfig config, Vector3 velocity)
        {
            Config = config;
            Velocity = velocity;
        }
    }
}