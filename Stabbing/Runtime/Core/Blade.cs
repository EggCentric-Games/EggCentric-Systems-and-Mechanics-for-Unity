using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EggCentric.Stabbing
{
    [RequireComponent(typeof(Collider2D))]
    public class Blade : MonoBehaviour
    {
        [SerializeField] private BladeParameters parameters;
        [SerializeField] private ContactFilter2D contactFilter;

        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private Collider2D bladeCollider;

        public Collider2D Collider => bladeCollider;
        public Vector2 EdgeDirection => transform.TransformDirection(parameters.EdgeDirection);
        public Vector2 CuttingDirection => parameters.IsVariableDirection ? rb.velocity : EdgeDirection;
        public Vector2 Velocity => rb.velocity;

        public event Action<BodyHit> OnBodyHit;

        private void OnEnable()
        {
            DeferRegistration();
        }

        private void OnDisable()
        {
            DeferUnregistration();
        }

        private void DeferRegistration()
        {
            if(StabbingSystem.IsValid)
                RegisterSelf();

            StabbingSystem.OnInitialized += RegisterSelf;
        }

        private void DeferUnregistration()
        {
            // If system is already initialized - unregister immediately
            if (StabbingSystem.IsValid)
                UnregisterSelf();

            // Otherwise - wait for system to initialize and then unregister
            StabbingSystem.OnInitialized += UnregisterSelf;
        }

        private void RegisterSelf()
        {
            StabbingSystem.Instance.RegisterBlade(this);
            StabbingSystem.OnInitialized -= RegisterSelf;
        }

        private void UnregisterSelf()
        {
            StabbingSystem.Instance.DeregisterBlade(this);
            StabbingSystem.OnInitialized -= UnregisterSelf;
        }

        private void FixedUpdate()
        {
            HandleIntersections();
        }

        private void HandleIntersections()
        {
            List<BodyHit> bodyHits = GetBodyHits();

            foreach (var bodyHit in bodyHits)
            {
                RegisterBodyHit(bodyHit);
            }
        }

        private void RegisterBodyHit(BodyHit bodyHit)
        {
            OnBodyHit?.Invoke(bodyHit);
        }

        private List<BodyHit> GetBodyHits()
        {
            List<RaycastHit2D> intersections = GetIntersections();

            return FilterBodyHits(intersections);
        }

        private List<RaycastHit2D> GetIntersections()
        {
            List<RaycastHit2D> intersections = new List<RaycastHit2D>();
            bladeCollider.Cast(Velocity, contactFilter, intersections, Velocity.magnitude * Time.deltaTime);

            return intersections.Where(x => !IsCollisionIgnored(x.collider)).ToList();
        }

        private List<BodyHit> FilterBodyHits(List<RaycastHit2D> intersections)
        {
            List<BodyHit> bodyHits = new List<BodyHit>();

            foreach (var hit in intersections)
            {
                if (hit.collider.TryGetComponent(out StabbableBody stabbableBody))
                {
                    BodyHit bodyHit = new BodyHit(hit, stabbableBody, this);
                    bodyHits.Add(bodyHit);
                }
            }

            return bodyHits;
        }

        private bool IsCollisionIgnored(Collider2D collider)
        {
            return Physics2D.GetIgnoreCollision(Collider, collider);
        }

        private void OnValidate()
        {
            bladeCollider ??= GetComponentInParent<Collider2D>();
            rb ??= GetComponentInParent<Rigidbody2D>();
        }
    }
}