using UnityEngine;


namespace EggCentric.Aerodynamics
{
    public class AerodynamicCoefficientCalculator : IAerodynamicCoefficientCalculator
    {
        private float _stallAnglePadding = 10f * Mathf.Deg2Rad;

        private IAerodynamicCalculationStrategy _lowAoAStrategy;
        private IAerodynamicCalculationStrategy _highAoAStrategy;

        public AerodynamicCoefficientCalculator()
        {
            _lowAoAStrategy = new LowAoAStrategy();
            _highAoAStrategy = new HighAoAStrategy(_lowAoAStrategy);
        }

        public AerodynamicCoefficients GetAerodynamicCoefficients(AerodynamicParameters aerodynamicParameters, float AoA, float aspectRatio)
        {
            float radianAoA = AoA * Mathf.Deg2Rad;

            if (Mathf.Abs(radianAoA) < aerodynamicParameters.StallAngle)
                return _lowAoAStrategy.GetAerodynamicCoefficients(aerodynamicParameters, radianAoA, aspectRatio);
            else if (Mathf.Abs(radianAoA) >= aerodynamicParameters.StallAngle + _stallAnglePadding)
                return _highAoAStrategy.GetAerodynamicCoefficients(aerodynamicParameters, radianAoA, aspectRatio);

            float lerpCoefficient = Mathf.Clamp01((Mathf.Abs(radianAoA) - aerodynamicParameters.StallAngle) / _stallAnglePadding);
            float smoothLerpCoefficient = (3f * lerpCoefficient * lerpCoefficient) - (2f * lerpCoefficient * lerpCoefficient * lerpCoefficient);

            AerodynamicCoefficients lowAoACoefficients = _lowAoAStrategy.GetAerodynamicCoefficients(aerodynamicParameters, radianAoA, aspectRatio);
            AerodynamicCoefficients highAoACoefficients = _highAoAStrategy.GetAerodynamicCoefficients(aerodynamicParameters, radianAoA, aspectRatio);

            AerodynamicCoefficients aerodynamicCoefficients = new AerodynamicCoefficients()
            {
                Lift = Mathf.Lerp(lowAoACoefficients.Lift, highAoACoefficients.Lift, smoothLerpCoefficient),
                Drag = Mathf.Lerp(lowAoACoefficients.Drag, highAoACoefficients.Drag, smoothLerpCoefficient),
                Torque = Mathf.Lerp(lowAoACoefficients.Torque, highAoACoefficients.Torque, smoothLerpCoefficient),
            };

            return aerodynamicCoefficients;
        }
    }
}