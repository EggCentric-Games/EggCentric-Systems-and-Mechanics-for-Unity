using FMODUnity;
using System.Collections.Generic;
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

    public enum ProjectileCollisionType
    {
        Entrance,
        Exit,
        Ricochet,
        Fracture,
        Elimination
    }

    public interface ISurface
    {

    }

    public interface ICollisionHandler<THit, TContact> where TContact : IContact
    {
        public TContact HandleHit(THit hit);
    }

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

    public interface IContact
    {

    }

    public struct DefaultContact : IContact
    {
        public ProjectileData Projectile { get; }
        public HitData Hit { get; }

        public DefaultContact(ProjectileData projectileData, HitData hitData)
        {
            Projectile = projectileData;
            Hit = hitData;
        }
    }

    
    public interface IContactInterpreterStack<TContact> : IContactInterpreter<TContact>, IItemStack<IContactInterpreter<TContact>> where TContact : IContact
    {

    }

    public class ContactInterpreterStack<TContact> : IContactInterpreterStack<TContact> where TContact : IContact
    {
        public IReadOnlyList<IContactInterpreter<TContact>> Items => _items;

        private readonly List<IContactInterpreter<TContact>> _items;

        public ContactInterpreterStack() => _items = new List<IContactInterpreter<TContact>>();

        public void Process(TContact contact)
        {
            foreach(var item in _items)
                item.Process(contact);
        }

        public void Add(IContactInterpreter<TContact> item) => _items.Add(item);
        public bool Remove(IContactInterpreter<TContact> item) => _items.Remove(item);
        public void Clear() => _items.Clear();
    }

    public interface IContactInterpreter<TContact> where TContact : IContact
    {
        public void Process(TContact contact);
    }

    public class ContactHandler<THit, TContact> where THit : struct where TContact : IContact
    {
        private ICollisionHandler<THit, TContact> _collisionHandler;
        private IContactInterpreter<TContact> _interpreter;

        public ContactHandler(ICollisionHandler<THit, TContact> collisionHandler, IContactInterpreter<TContact> interpreter)
        {
            _collisionHandler = collisionHandler;
            _interpreter = interpreter;
        }

        public void HandleHit(THit hit)
        {
            var contact = _collisionHandler.HandleHit(hit);
            _interpreter.Process(contact);
        }
    }

    public class DefaultSFXInterpretter : IContactInterpreter<DefaultContact>
    {
        private IRuleSet<DefaultContact, DefaultSFXRequest> _ruleSet;

        public DefaultSFXInterpretter(IRuleSet<DefaultContact, DefaultSFXRequest> ruleSet) => _ruleSet = ruleSet;

        public void Process(DefaultContact contact)
        {
            var sfxRequest = _ruleSet.GetRequest(contact);
            ExecuteRequest(sfxRequest);
        }

        private void ExecuteRequest(DefaultSFXRequest request) => RuntimeManager.PlayOneShot(request.Event, request.Position);
    }

    public interface IRequest
    {

    }

    public interface IRuleSet<TContact, TRequest> where TContact : IContact where TRequest : IRequest
    {
        public TRequest GetRequest(TContact contact);
    }

    public struct DefaultSFXRequest : IRequest
    {
        public EventReference Event { get; }
        public Vector3 Position { get; }

        public DefaultSFXRequest(EventReference @event, Vector3 position)
        {
            Event = @event;
            Position = position;
        }
    }

    public class DefaultVFXInterpretter : IContactInterpreter<DefaultContact>
    {
        private IRuleSet<DefaultContact, DefaultVFXRequest> _ruleSet;

        public DefaultVFXInterpretter(IRuleSet<DefaultContact, DefaultVFXRequest> ruleSet) => _ruleSet = ruleSet;

        public void Process(DefaultContact contact)
        {
            var vfxRequest = _ruleSet.GetRequest(contact);
            ExecuteRequest(vfxRequest);
        }

        private void ExecuteRequest(DefaultVFXRequest request) => GameObject.Instantiate(request.Prefab, request.Position, request.Rotation);
    }

    public struct DefaultVFXRequest : IRequest
    {
        public ParticleSystem Prefab { get; }
        public Vector3 Position { get; }
        public Quaternion Rotation { get; }

        public DefaultVFXRequest(ParticleSystem prefab, Vector3 position, Quaternion rotation)
        {
            Prefab = prefab;
            Position = position;
            Rotation = rotation;
        }
    }
}