using UnityEngine;


namespace EggCentric.Aerodynamics
{
    [System.Serializable]
    public struct AerodynamicParameters
    {
        public float LiftSlope2D => _liftSlope2D;
        public float SurfaceFrictionFactor => _surfaceFrictionFactor;
        public float OswaldFactor => _oswaldFactor;
        public float ZeroLiftAoA => _zeroLiftAoA * Mathf.Deg2Rad;
        public float StallAngle => _stallAngle * Mathf.Deg2Rad;

        [Tooltip("How fast lift force increases\n~2 * Pi for a thin airfoil")]
        [SerializeField] private float _liftSlope2D; // ~2 * Pi for a thin airfoil
        [Tooltip("Parasite drag of the surface\n~0.02-0.03 for basic wings")]
        [SerializeField] private float _surfaceFrictionFactor; // 0.02-0.03 for basic wings
        [Tooltip("Oswald efficiency number\n~0.7-0.9 for basic wings")]
        [SerializeField] private float _oswaldFactor; // ~0.7–0.9
        [Tooltip("Angle at which lift surface produces zero lift")]
        [SerializeField] private float _zeroLiftAoA;
        [Tooltip("Angle of max lift force and rapid drag increase\nUsually is 10-15 degrees")]
        [SerializeField] private float _stallAngle;
    }
}