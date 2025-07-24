using EggCentric.Infrastructure;
using EggCentric.Singletons;
using System.Collections.Generic;
using UnityEngine;

namespace EggCentric.Stabbing
{
    public class StabbingSystem : Singleton<StabbingSystem>
    {
        private List<Blade> blades = new();
        private List<Stab> stabs = new();
        private ILifeCycleProvider updater;

        public StabbingSystem(ILifeCycleProvider updater) : base()
        {
            this.updater = updater;
        }

        public void RegisterBlade(Blade blade)
        {
            if (blades.Contains(blade))
                return;

            blades.Add(blade);
            blade.OnBodyHit += RegisterStab;
        }

        public void DeregisterBlade(Blade blade)
        {
            if (blades.Contains(blade))
                return;

            blades.Remove(blade);
            blade.OnBodyHit -= RegisterStab;
        }

        private void RegisterStab(BodyHit hit)
        {
            Debug.Log($"{hit.Blade.Collider.name} hitted: {hit.Body.Collider.name}");

            //SliderJoint2D joint = Utilities.CreateSliderJoint(hit.Blade.Collider.attachedRigidbody, hit.Body.Collider.attachedRigidbody, hit.Contact.point, hit.Blade.CuttingDirection);

            FrictionJointConfig config = CreateFrictionConfig(hit);
            FrictionJoint2D joint = Utilities.CreateFrictionJoint(config);
            Stab stab = new Stab(updater, hit.Blade.Collider, hit.Body.Collider, joint);
            stabs.Add(stab);
            stab.OnBladeLeavesBody += DestroyStab;
            stab.OnJointOverstressed += DestroyStab;
            Physics2D.IgnoreCollision(hit.Blade.Collider, hit.Body.Collider);
        }

        private void DestroyStab(Stab stab)
        {
            stabs.Remove(stab);
            stab.Remove();
            stab.OnBladeLeavesBody -= DestroyStab;
            stab.OnJointOverstressed -= DestroyStab;
            GameObject.Destroy(stab.Connection);
            Physics2D.IgnoreCollision(stab.BladeCollider, stab.BodyCollider, false);
        }

        private FrictionJointConfig CreateFrictionConfig(BodyHit hit)
        {
            return new FrictionJointConfig(hit.Blade.Collider.attachedRigidbody, hit.Body.Collider.attachedRigidbody, hit.Contact.point)
            {
                AutoAnchor = false,
                EnableCollision = true,
                JointBreakAction = JointBreakAction2D.Ignore,
                ForceDrag = hit.Body.Density,
                TorqueDrag = hit.Body.Density,
            };
        }
    }
}