using UnityEngine;

namespace EggCentric.Ballistics
{
    public class DefaultCollisionHandler : ICollisionHandler<RaycastHit, DefaultContact>
    {
        private IProjectile _attachedProjectile;
        private IAngleBasedEvaluator _richochetEvaluator;

        public DefaultCollisionHandler(IProjectile attachedProjectile)
        {
            _attachedProjectile = attachedProjectile;
            _richochetEvaluator = new LerpBasedEvaluator();
        }

        public DefaultContact HandleHit(RaycastHit hit)
        {
            var projectileData = new ProjectileData(_attachedProjectile.Config, _attachedProjectile.Velocity);
            var surfaceType = GetSurfaceType();
            var hitType = GetHitType(projectileData, hit, surfaceType);

            var hitData = new HitData(hit.transform, hit.point, hit.normal, surfaceType, hitType);
            var contact = new DefaultContact(projectileData, hitData);

            return contact;
        }

        private ProjectileCollisionType GetHitType(ProjectileData projectileData, RaycastHit hit, ISurface surfaceType)
        {
            var hitAngle = Vector3.Angle(projectileData.Velocity, -hit.normal);
            var ricochetChance = _richochetEvaluator.Evaluate(hitAngle, projectileData.Config.Ricochetness);

            if (Random.value < ricochetChance)
                return ProjectileCollisionType.Ricochet;

            return ProjectileCollisionType.Entrance;
        }

        private ISurface GetSurfaceType()
        {
            return null;
        }
    }
}