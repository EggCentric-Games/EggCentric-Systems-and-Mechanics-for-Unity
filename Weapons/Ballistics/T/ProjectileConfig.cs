using System;
using UnityEngine;
using EggCentric.QoL.Selectors;

namespace EggCentric.Ballistics
{
    [CreateAssetMenu(fileName = "ProjectileConfig_", menuName = "Configs/Projectile")]
    public class ProjectileConfig : ScriptableObject
    {
        public RangeValueSelector Speed => _speed;
        public RangeValueSelector Size => _size;
        public FixedValueSelector Gravity => _gravity;
        public FixedValueSelector Knockback => _knockback;
        public float Ricochetness => _ricochetness;
        public float Bounciness => _bounciness;

        [SerializeField] private RangeValueSelector _speed;
        [SerializeField] private RangeValueSelector _size;
        [SerializeField] private FixedValueSelector _gravity;
        [SerializeField] private FixedValueSelector _knockback;
        [Range(0f, 1f)]
        [SerializeField] private float _ricochetness;
        [Range(0f, 1f)]
        [SerializeField] private float _bounciness;
    }
}