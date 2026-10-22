using UnityEngine;
using EggCentric.Common.Behaviours;

namespace EggCentric.Ballistics
{
    public interface IKinematic : IGameEntity
    {
        public Vector3 Velocity { get; }
    }
}