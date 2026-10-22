using EggCentric.Emmiters;
using EggCentric.Randomization;
using UnityEngine;

namespace EggCentric.Ballistics
{
    public class Emitter : IEmitter
    {
        public float Spread;
        private IRandom _random;

        public Emitter(float spread = 360f) : this(spread, RandomTypes.Uniform) { }
        public Emitter(float spread, IRandom random)
        {
            Spread = spread;
            _random = random;
        }

        public Point Emit()
        {
            return new Point(Vector3.zero, Quaternion.AngleAxis(_random.Range(0, Spread), Vector3.forward) * Vector3.right);
        }
    }

    public class Shotgun2D : MonoBehaviour
    {
        [SerializeField] private Bullet2D _bulletPrefab;
        [SerializeField] private Transform _barrel;
        [SerializeField] private float _spread;
        [SerializeField] private int _bulletCount;

        private Quaternion _offset;
        private Emitter _emitter;

        private void Awake()
        {
            _emitter = new Emitter(_spread, RandomTypes.Uniform);
            _offset = Quaternion.AngleAxis(-_spread / 2f, Vector3.forward);
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
                Shoot();
        }

        private void Shoot()
        {
            for (int i = 0; i < _bulletCount; i++)
                SpawnBullet();
        }

        private void SpawnBullet()
        {
            var spawnDirection = _emitter.Emit().Direction;
            var bullet = Instantiate(_bulletPrefab, transform.position, _offset * Quaternion.FromToRotation(Vector3.right, spawnDirection), transform);
        }
    }
}