using EggCentric.EffectsHandling.VFX;
using UnityEngine;

namespace EggCentric.Ballistics
{
    public class DefaultVFXInterpretter : IContactInterpreter<DefaultContact>
    {
        private IRuleSet<DefaultContact, DefaultVFXRequest> _ruleSet;

        public DefaultVFXInterpretter(IRuleSet<DefaultContact, DefaultVFXRequest> ruleSet) => _ruleSet = ruleSet;

        public void Process(DefaultContact contact)
        {
            var vfxRequest = _ruleSet.RetrieveFor(contact);
            ExecuteRequest(vfxRequest);
        }

        private void ExecuteRequest(DefaultVFXRequest request) => GameObject.Instantiate(request.Config.Prefab, request.Position, request.Rotation);
    }
}