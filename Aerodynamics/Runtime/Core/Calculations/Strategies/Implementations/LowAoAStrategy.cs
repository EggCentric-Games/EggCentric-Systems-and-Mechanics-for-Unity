using UnityEngine;


namespace EggCentric.Aerodynamics
{
    public class LowAoAStrategy : AerodynamicCalculationStrategy
    {
        public override AerodynamicCoefficients GetAerodynamicCoefficients(AerodynamicParameters aerodynamicParameters, float AoA, float aspectRatio)
        {
            float liftCoefficient = GetLiftCoefficient(aerodynamicParameters, AoA, aspectRatio);

            float inducedAngle = GetInducedAngle(liftCoefficient, aspectRatio, aerodynamicParameters.OswaldFactor);
            float effectiveAngle = GetEffectiveAngle(AoA, inducedAngle, aerodynamicParameters.ZeroLiftAoA);

            float tangentialCoefficient = aerodynamicParameters.SurfaceFrictionFactor * Mathf.Cos(effectiveAngle);
            float normalCoefficient = (liftCoefficient + (tangentialCoefficient * Mathf.Sin(effectiveAngle))) / Mathf.Cos(effectiveAngle);

            float dragCoefficient = GetDragCoefficient(tangentialCoefficient, normalCoefficient, effectiveAngle);
            float pitchingMomentCoefficient = GetPitchingMomentCoefficient(normalCoefficient, effectiveAngle);

            AerodynamicCoefficients aerodynamicCoefficients = new AerodynamicCoefficients()
            {
                Lift = liftCoefficient,
                Drag = dragCoefficient,
                Torque = pitchingMomentCoefficient
            };

            return aerodynamicCoefficients;
        }

        private float GetLiftCoefficient(AerodynamicParameters aerodynamicParameters, float AoA, float aspectRatio)
        {
            float vortexCorrection = aspectRatio / (aspectRatio + 2 * (aspectRatio + 4) / (aspectRatio + 2));
            float rawLiftCoefficient = aerodynamicParameters.LiftSlope2D * (AoA - aerodynamicParameters.ZeroLiftAoA);
            float liftCoefficient = rawLiftCoefficient * vortexCorrection;

            return liftCoefficient;
        }
    }
}