using UnityEngine;


namespace EggCentric.Aerodynamics
{
    public abstract class AerodynamicCalculationStrategy : IAerodynamicCalculationStrategy
    {
        public abstract AerodynamicCoefficients GetAerodynamicCoefficients(AerodynamicParameters aerodynamicParameters, float AoA, float aspectRatio);

        protected float GetDragCoefficient(float tangentialCoefficient, float normalCoefficient, float effectiveAngle)
        {
            float dragCoefficient = normalCoefficient * Mathf.Sin(effectiveAngle) + tangentialCoefficient * Mathf.Cos(effectiveAngle);

            return dragCoefficient;
        }

        protected float GetPitchingMomentCoefficient(float normalCoefficient, float effectiveAngle)
        {
            float aerodynamicCenter = 0.25f; // relative distance from leading edge to force application point ranges from 0 to 1  
            float pitchingCorrection = -0.175f * (1 - (2 * effectiveAngle / Mathf.PI));
            float pitchingMomentCoefficient = -normalCoefficient * (aerodynamicCenter + pitchingCorrection);

            return pitchingMomentCoefficient;
        }

        protected float GetInducedAngle(float liftCoefficient, float aspectRatio, float oswaldFactor)
        {
            return liftCoefficient / (Mathf.PI * aspectRatio * oswaldFactor);
        }

        protected float GetEffectiveAngle(float AoA, float inducedAngle, float zeroLiftAoA)
        {
            return AoA - inducedAngle - zeroLiftAoA;
        }
    }
}