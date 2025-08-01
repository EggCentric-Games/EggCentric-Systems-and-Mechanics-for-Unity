using UnityEngine;

[CreateAssetMenu(fileName = "AerodynamicConfig_", menuName = "EggCentric/Aerodynamics/Config")]
public class AerodynamicConfig : ScriptableObject
{
    public float AirDensity => airDensity;

    [SerializeField] private float airDensity;
}