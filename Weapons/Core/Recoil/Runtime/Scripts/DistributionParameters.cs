using UnityEngine;

[System.Serializable]
public struct DistributionParameters
{
    public AnimationCurve DistributionCurve;
    public float ValueMultiplier;
    public float DistributionTime;

    public DistributionParameters(AnimationCurve distributionCurve, float valueMultiplier, float distributionTime)
    {
        DistributionCurve = distributionCurve;
        ValueMultiplier = valueMultiplier;
        DistributionTime = distributionTime;
    }
}
