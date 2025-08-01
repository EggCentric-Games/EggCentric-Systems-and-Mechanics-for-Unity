using UnityEngine;


namespace EggCentric.Aerodynamics
{
    public class HighAoAStrategy : AerodynamicCalculationStrategy
    {
        private const float _FlatPlateDragCoefficient = 1.98f;

        private IAerodynamicCalculationStrategy _preStallCalulationStrategy;

        public HighAoAStrategy(IAerodynamicCalculationStrategy preStallCalulationStrategy)
        {
            _preStallCalulationStrategy = preStallCalulationStrategy;
        }

        public override AerodynamicCoefficients GetAerodynamicCoefficients(AerodynamicParameters aerodynamicParameters, float AoA, float aspectRatio)
        {
            float maxLiftAoA = Mathf.Clamp(AoA, -aerodynamicParameters.StallAngle, aerodynamicParameters.StallAngle);
            float stallLiftCoefficient = _preStallCalulationStrategy.GetAerodynamicCoefficients(aerodynamicParameters, maxLiftAoA, aspectRatio).Lift;

            float inducedAngle = GetInducedAngle(stallLiftCoefficient, aspectRatio, aerodynamicParameters.OswaldFactor);

            float tapering = Mathf.InverseLerp(aerodynamicParameters.StallAngle, Mathf.PI / 2f, Mathf.Abs(AoA));
            float taperedAngle = Mathf.Lerp(inducedAngle, 0, tapering);
            float effectiveAngle = GetEffectiveAngle(AoA, taperedAngle, aerodynamicParameters.ZeroLiftAoA);

            float tangentialCoefficient = 0.5f * aerodynamicParameters.SurfaceFrictionFactor * Mathf.Cos(effectiveAngle);
            float normalCoefficient = _FlatPlateDragCoefficient * Mathf.Sin(effectiveAngle) * ((1 / (0.56f + 0.44f * Mathf.Sin(Mathf.Abs(effectiveAngle)))) - (0.41f * (1 - Mathf.Exp(-17f / aspectRatio))));

            Debug.Log($"L:{stallLiftCoefficient};\nI: {inducedAngle}; At: {taperedAngle}; E: {effectiveAngle};\nT: {tangentialCoefficient}; N: {normalCoefficient};");

            float liftCoefficient = GetLiftCoefficient(tangentialCoefficient, normalCoefficient, effectiveAngle);

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

        private float GetLiftCoefficient(float tangentialCoefficient, float normalCoefficient, float effectiveAngle)
        {
            float liftCoefficient = normalCoefficient * Mathf.Cos(effectiveAngle) - tangentialCoefficient * Mathf.Sin(effectiveAngle);

            return liftCoefficient;
        }
    }
}