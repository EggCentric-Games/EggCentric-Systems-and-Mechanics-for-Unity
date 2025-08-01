namespace EggCentric.Aerodynamics
{
    public interface IAerodynamicCalculationStrategy
    {
        public AerodynamicCoefficients GetAerodynamicCoefficients(AerodynamicParameters aerodynamicParameters, float AoA, float aspectRatio);
    }
}