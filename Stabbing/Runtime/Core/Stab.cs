using System;
using UnityEngine;
using EggCentric.Geometry.Intersections;
using EggCentric.LifeCycleHandling;

namespace EggCentric.Stabbing
{
    public class Stab
    {
        protected Collider2D bladeCollider;
        protected Collider2D bodyCollider;
        //protected SliderJoint2D connection;
        protected FrictionJoint2D connection;

        protected ILifeCycleProvider updater;

        private Intersector intersector;

        public Collider2D BladeCollider => bladeCollider;
        public Collider2D BodyCollider => bodyCollider;
        //public SliderJoint2D Connection => connection;
        public FrictionJoint2D Connection => connection;

        public event Action<Stab> OnBladeLeavesBody;
        public event Action<Stab> OnJointOverstressed;

        public Stab(ILifeCycleProvider updater, Collider2D bladeCollider, Collider2D bodyCollider, FrictionJoint2D connection)
        {
            this.bladeCollider = bladeCollider;
            this.bodyCollider = bodyCollider;
            this.updater = updater;
            this.connection = connection;

            intersector = new ObjectIntersector(bladeCollider);

            updater.OnFixedUpdatePerformed += OnUpdate;
        }

        public void Remove()
        {
            updater.OnFixedUpdatePerformed -= OnUpdate;
        }

        protected void OnUpdate()
        {
            if (HandleTranslationLength())
                return;

            if (HandleJointStress())
                return;

            UpdateJointValues();
        }

        protected void UpdateJointValues()
        {
            Intersection intersection = intersector.GetIntersection(bodyCollider);

            connection.anchor = bladeCollider.transform.InverseTransformPoint(intersection.Center);
            connection.connectedAnchor = bodyCollider.transform.InverseTransformPoint(intersection.Center);
        }

        protected bool HandleTranslationLength()
        {
            if (bladeCollider.Distance(bodyCollider).distance <= 0)
                return false;

            OnBladeLeavesBody?.Invoke(this);
            return true;
        }

        protected bool HandleJointStress()
        {
            if (!IsForceOverLimit() && !IsTorqueOverLimit())
                return false;

            OnJointOverstressed?.Invoke(this);
            return true;
        }

        protected bool IsForceOverLimit()
        {
            return connection.reactionForce.magnitude > connection.breakForce;
        }

        protected bool IsTorqueOverLimit()
        {
            return connection.reactionTorque > connection.breakTorque;
        }
    }

    public class SlidingStab : Stab
    {
        public SlidingStab(ILifeCycleProvider updater, Collider2D bladeCollider, Collider2D bodyCollider, FrictionJoint2D connection) : base(updater, bladeCollider, bodyCollider, connection)
        {
        }
    }

    public class FixedStab : Stab
    {
        public FixedStab(ILifeCycleProvider updater, Collider2D bladeCollider, Collider2D bodyCollider, FrictionJoint2D connection) : base(updater, bladeCollider, bodyCollider, connection)
        {
        }
    }
}