using EggCentric.Randomization;
using UnityEngine;

namespace EggCentric.Ballistics
{
    public class TestRig : MonoBehaviour
    {
        [SerializeField] private ProjectileSettings _projectileSettings;
        [SerializeField] private int _bulletsCount;
        [SerializeField] private float _spread;

        private ConeEmitter _emitter;
        private Penetrator[] _projectiles;

        private void Awake()
        {
            _projectiles = new Penetrator[_bulletsCount];
            _emitter = new ConeEmitter(_spread, RandomTypes.Gaussian);

            for (int i = 0; i < _bulletsCount; i++)
                CreatePenetrator(i);
        }

        private void CreatePenetrator(int i)
        {
            var randomDeviation = Random.insideUnitCircle;
            var projectile = new Penetrator(_projectileSettings, transform.position, -_emitter.Emit().Direction);
            _projectiles[i] = projectile;
        }

        private void OnDrawGizmos()
        {
            if (_projectiles == null)
                return;

            foreach (var projectile in _projectiles)
                projectile?.Draw();
        }
    }
}