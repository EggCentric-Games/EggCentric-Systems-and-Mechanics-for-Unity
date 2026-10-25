using EggCentric.EffectsHandling.VFX;
using UnityEngine;

namespace EggCentric.Ballistics
{
    [CreateAssetMenu(fileName = "VfxRuleSet_", menuName = "EggCentric/RuleSets/VFX")]
    public class DefaultVFXRuleSet : ScriptableObject, IRuleSet<DefaultContact, DefaultVFXRequest>
    {
        [SerializeField] private DefaultVFXConfig _hitEffect;

        public DefaultVFXRequest RetrieveFor(DefaultContact contact)
        {
            var VFXDirection = Vector3.Reflect(contact.Projectile.Velocity, contact.Hit.Normal);
            var hitRotation = Quaternion.FromToRotation(Vector3.up, VFXDirection);

            return new DefaultVFXRequest(_hitEffect, contact.Hit.Point, hitRotation);
        }
    }
}