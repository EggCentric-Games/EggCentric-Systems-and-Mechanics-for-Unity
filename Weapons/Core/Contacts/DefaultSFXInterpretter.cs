using EggCentric.EffectsHandling;
using System;

namespace EggCentric.Ballistics
{
    public class DefaultSFXInterpretter<TConfig, TRequest> : IContactInterpreter<DefaultContact> where TConfig : IEffectConfig where TRequest : EffectRequest<TConfig>
    {
        private IEffectPlayer<TRequest, IEffectHandle> _player;
        private IRuleSet<DefaultContact, TConfig> _ruleSet;
        private Func<TConfig, TRequest> _requestMaker;

        public DefaultSFXInterpretter(IEffectPlayer<TRequest, IEffectHandle> player, IRuleSet<DefaultContact, TConfig> ruleSet, Func<TConfig, TRequest> requestMaker)
        {
            _player = player;
            _ruleSet = ruleSet;
            _requestMaker = requestMaker;
        }

        public void Process(DefaultContact contact)
        {
            var config = _ruleSet.RetrieveFor(contact);
            var request = _requestMaker(config);
            _player.PlayOnce(request);
        }
    }
}