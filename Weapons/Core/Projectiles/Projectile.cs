using EggCentric.EffectsHandling.SFX;
using EggCentric.EffectsHandling.SFX.FMOD;
using UnityEngine;

namespace EggCentric.Ballistics
{
    public class Projectile : MonoBehaviour, IProjectile, IContactInterpreter<DefaultContact>
    {
        public ProjectileConfig Config => config;
        public Vector3 Position => transform.position;
        public Vector3 Velocity { get; protected set; }

        [SerializeField] protected ProjectileConfig config;

        [SerializeField] private DefaultSFXRuleSet _sfxRuleSet;
        [SerializeField] private DefaultVFXRuleSet _vfxRuleSet;
        [SerializeField] private CollisionProbeComponent _collisionProbe;

        private ContactHandler<RaycastHit, DefaultContact> _contactHandler;

        public virtual void Tick(float timeStep)
        {
            _collisionProbe.Tick(timeStep);
            Translate(timeStep);

            Velocity += Vector3.down * config.Gravity.Value * timeStep;
        }

        public virtual void Process(DefaultContact contact) { Destroy(gameObject); }

        protected virtual void Awake()
        {
            _collisionProbe.Initialize(this);
            var collisionHandler = new DefaultCollisionHandler(this);
            var interpreter = new ContactInterpreterStack<DefaultContact>();
            interpreter.Add(this);
            interpreter.Add(new DefaultSFXInterpretter<DefaultFMODSampleConfig, DefaultFMODRequest>(new NaiveFMODPlayer(), _sfxRuleSet, (config) => { return new DefaultFMODRequest(config, Position); } ));
            interpreter.Add(new DefaultVFXInterpretter(_vfxRuleSet));

            _contactHandler = new ContactHandler<RaycastHit, DefaultContact>(collisionHandler, interpreter);
            _collisionProbe.OnHit += _contactHandler.HandleHit;

            Velocity = transform.forward * config.Speed.Value;
        }

        private void Update() => Tick(Time.deltaTime);

        private void Translate(float timeStep) => transform.position += Velocity * timeStep;
    }
}