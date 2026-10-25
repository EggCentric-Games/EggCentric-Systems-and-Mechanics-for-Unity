using EggCentric.Emmiters;
using EggCentric.Randomization;
using UnityEngine;

namespace EggCentric.Ballistics
{
    public class ConeEmitter : IEmitter
    {
        private readonly IRandom _random;
        private readonly IDeviationStrategy _deviationStrategy;

        private float _coneAngle;

        public ConeEmitter(float spread, IRandom random)
        {
            _coneAngle = spread / 2f;
            _random = random;

            _deviationStrategy = new EdgeDeviation();
        }

        public Point Emit()
        {
            var t = _deviationStrategy.GetValue(_random);
            var maxCos = Mathf.Cos(_coneAngle * Mathf.Deg2Rad);
            var cos = Mathf.Lerp(1, maxCos, t);

            var targetAngle = Mathf.Acos(cos) * Mathf.Rad2Deg;
            var deviatedDirection = Quaternion.AngleAxis(targetAngle, Vector3.right) * Vector3.forward;
            var inverted = Random.value > 0.5f;

            if (inverted)
                deviatedDirection.y *= -1f;

            var circularDeviation = Random.Range(0f, 180f);
            var rotationalModifier = Quaternion.AngleAxis(circularDeviation, Vector3.forward);

            return new Point(Vector3.zero, rotationalModifier * deviatedDirection);
        }
    }

    public class Shotgun3D : MonoBehaviour
    {
        [SerializeField] private TestAmmoConfig _ammoConfig;
        [SerializeField] private Transform _barrel;
        [SerializeField] private float _spread;

        private ConeEmitter _emitter;

        private void Awake()
        {
            _emitter = new ConeEmitter(_spread, RandomTypes.Gaussian);
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
                Shoot();
        }

        private void Shoot()
        {
            for (int i = 0; i < _ammoConfig.ProjectileCount; i++)
                SpawnBullet();
        }

        private void SpawnBullet()
        {
            var spawnDirection = transform.rotation * _emitter.Emit().Direction;
            var bullet = Instantiate(_ammoConfig.ProjectilePrefab, transform.position, Quaternion.FromToRotation(Vector3.forward, spawnDirection), transform);
        }
    }
}