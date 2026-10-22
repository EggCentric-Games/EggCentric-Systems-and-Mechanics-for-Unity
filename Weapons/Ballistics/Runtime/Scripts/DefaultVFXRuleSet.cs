using UnityEngine;

namespace EggCentric.Ballistics
{
    [CreateAssetMenu(fileName = "VfxRuleSet_", menuName = "EggCentric/RuleSets/VFX")]
    public class DefaultVFXRuleSet : ScriptableObject, IRuleSet<DefaultContact, DefaultVFXRequest>
    {
        [SerializeField] private ParticleSystem _hit;

        public DefaultVFXRequest GetRequest(DefaultContact contact)
        {
            var VFXDirection = Vector3.Reflect(contact.Projectile.Velocity, contact.Hit.Normal);
            var hitRotation = Quaternion.FromToRotation(Vector3.up, VFXDirection);

            return new DefaultVFXRequest(_hit, contact.Hit.Point, hitRotation);
        }
    }
}