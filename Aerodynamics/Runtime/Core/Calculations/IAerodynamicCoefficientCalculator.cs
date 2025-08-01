
namespace EggCentric.Aerodynamics
{
    public interface IAerodynamicCoefficientCalculator
    {
        public AerodynamicCoefficients GetAerodynamicCoefficients(AerodynamicParameters aerodynamicParameters, float AoA, float aspectRatio);
    }
}