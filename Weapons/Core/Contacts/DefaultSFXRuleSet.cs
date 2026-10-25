using EggCentric.Ballistics;
using EggCentric.EffectsHandling.SFX.FMOD;
using UnityEngine;

namespace EggCentric.EffectsHandling.SFX
{
    [CreateAssetMenu(fileName = "SfxRuleSet_", menuName = "EggCentric/RuleSets/SFX")]
    public class DefaultSFXRuleSet : ScriptableObject, IRuleSet<DefaultContact, DefaultFMODSampleConfig>
    {
        [SerializeField] private DefaultFMODSampleConfig _sfxSample;

        public DefaultFMODSampleConfig RetrieveFor(DefaultContact contact) => _sfxSample;
    }
}