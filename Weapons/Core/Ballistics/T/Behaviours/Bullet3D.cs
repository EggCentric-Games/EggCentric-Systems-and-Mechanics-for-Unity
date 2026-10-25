using UnityEngine;

namespace EggCentric.Ballistics
{
    public interface IAngleBasedEvaluator
    {
        public float Evaluate(float angle, float value);
    }

    public class StaticEvaluator : IAngleBasedEvaluator
    {
        public float Evaluate(float _, float value) => value;
    }

    public class AngleBasedEvaluator : IAngleBasedEvaluator
    {
        public float Evaluate(float angle, float value) => angle / 180f * value;
    }

    public class LerpBasedEvaluator : IAngleBasedEvaluator
    {
        public float Evaluate(float angle, float value)
        {
            var t = angle / 180f;
            return Mathf.Lerp(value, 1f, t);
        }
    }

    public class Bullet3D : Bullet
    {
        private IAngleBasedEvaluator _bounceEvaluator;

        protected override void Awake()
        {
            base.Awake();
            _bounceEvaluator = new LerpBasedEvaluator();
        }

        private void FixedUpdate() => Tick(Time.fixedDeltaTime);

        public override void Process(DefaultContact contact)
        {
            var bulletMass = 0.01f;

            switch(contact.Hit.HitType)
            {
                case ProjectileCollisionType.Ricochet:
                    {
                        var hitAngle = Vector3.Angle(contact.Projectile.Velocity, -contact.Hit.Normal);
                        var remainingVelocity = _bounceEvaluator.Evaluate(hitAngle, config.Bounciness);
                        var absorbedVelocity = Velocity * (1f - remainingVelocity);

                        Velocity = Vector3.Reflect(Velocity * remainingVelocity, contact.Hit.Normal);
                        ApplyKnockback(contact, absorbedVelocity.magnitude * bulletMass);
                        break;
                    }
                default:
                    {
                        var decalSize = config.Size.Value;
                        spawner.SpawnHitmark(contact.Hit.Object, contact.Hit.Point, contact.Hit.Normal, decalSize);
                        Destroy(gameObject);

                        ApplyKnockback(contact, Velocity.magnitude * bulletMass);
                        break;
                    }
            }
            
        }

        public override void Tick(float timeStep)
        {
            base.Tick(timeStep);
            AlignWithSpeed();
        }

        private void AlignWithSpeed()
        {
            transform.LookAt(transform.position + Velocity);
        }

        private void ApplyKnockback(DefaultContact contact, float force)
        {
            var hitBody = contact.Hit.Object.GetComponentInParent<Rigidbody>();
            if (hitBody == null)
                return;

            var forceDirection = -contact.Hit.Normal;
            hitBody.AddForceAtPosition(forceDirection * force * config.Knockback.Value, contact.Hit.Point, ForceMode.Impulse);
        }
    }
}