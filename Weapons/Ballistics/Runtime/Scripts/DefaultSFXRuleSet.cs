using FMODUnity;
using UnityEngine;

namespace EggCentric.Ballistics
{
    [CreateAssetMenu(fileName = "SfxRuleSet_", menuName = "EggCentric/RuleSets/SFX")]
    public class DefaultSFXRuleSet : ScriptableObject, IRuleSet<DefaultContact, DefaultSFXRequest>
    {
        [SerializeField] private EventReference _hit;

        public DefaultSFXRequest GetRequest(DefaultContact contact) => new DefaultSFXRequest(_hit, contact.Hit.Point);
    }
}