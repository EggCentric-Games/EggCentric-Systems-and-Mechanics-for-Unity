using EggCentric.Randomization;
using UnityEngine;

namespace EggCentric.Ballistics
{
    public class Grenade : MonoBehaviour
    {
        [SerializeField] private Bullet3D _bulletPrefab;
        [SerializeField] private Transform _barrel;
        [SerializeField] private float _spread;
        [SerializeField] private int _bulletCount;
        [SerializeField] private float _spawnOffset;

        private ConeEmitter _emitter;

        private void Awake()
        {
            _emitter = new ConeEmitter(_spread, RandomTypes.Uniform);
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
            var bullet = Instantiate(_bulletPrefab, transform.position + spawnDirection * _spawnOffset, Quaternion.FromToRotation(Vector3.forward, spawnDirection), null);
        }
    }
}