using EggCentric.LifeCycleHandling;
using EggCentric.Sensors;
using EggCentric.ValueProviders;
using System;
using UnityEngine;

namespace EggCentric.Ballistics
{
    public class CollisionProbeComponent : MonoBehaviour, ITickable
    {
        [SerializeField] private ProbeSettings3D _raycastLayers;

        private IProjectile _projectile;
        private CollisionProbe<RaycastHit> _collisionProbe;

        public event Action<RaycastHit> OnHit
        {
            add => _collisionProbe.OnContact += value;
            remove => _collisionProbe.OnContact -= value;
        }

        public void Initialize(IProjectile projectile)
        {
            _projectile = projectile;
            _collisionProbe = new CollisionProbe<RaycastHit>(new RaycastProbe3D(_raycastLayers), new Getter<Vector3>(() => _projectile.Position), new Getter<Vector3>(() => _projectile.Velocity));

            Debug.Log($"Initialized: {_collisionProbe}");
        }

        public void Tick(float timeStep) => _collisionProbe.Tick(timeStep);
    }
}

