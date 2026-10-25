using UnityEngine;

namespace EggCentric.Ballistics
{
    public struct HitData
    {
        public Transform Object { get; }
        public Vector3 Point { get; }
        public Vector3 Normal { get; }
        public ISurface Surface { get; }
        public ProjectileCollisionType HitType { get; }

        public HitData(Transform @object, Vector3 point, Vector3 normal, ISurface surface, ProjectileCollisionType hitType)
        {
            Object = @object;
            Point = point;
            Normal = normal;
            Surface = surface;
            HitType = hitType;
        }
    }
}