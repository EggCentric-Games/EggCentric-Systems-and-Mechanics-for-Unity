using EggCentric.QoL;
using UnityEngine;

[System.Serializable]
public class DistributedValue
{
    public float DistributionTime => _distributionTime;

    private AnimationCurve _distributionCurve;
    private float _distributionTime;
    private float _valueMultiplier;
    private float _curveNormalization;

    public DistributedValue(DistributionParameters distributionParameters)
    {
        _distributionCurve = distributionParameters.DistributionCurve;
        _distributionTime = distributionParameters.DistributionTime;
        _valueMultiplier = distributionParameters.ValueMultiplier;

        _distributionCurve.RemapTo(Vector2.up, Vector2.up);
        _curveNormalization = GetNormalizationValue(_distributionCurve);
    }

    private float GetNormalizationValue(AnimationCurve curve)
    {
        var curveArea = curve.GetCurveArea(4096);
        return _valueMultiplier / (curveArea * _distributionTime);
    }

    public float EvaluateAt(float time)
    {
        var normalizedTime = time / _distributionTime;
        return _distributionCurve.Evaluate(normalizedTime);
    }

    public float GetDistributedValue(float time, float timeStep)
    {
        var deltaTime = Mathf.Clamp(timeStep, 0, _distributionTime - time);
        var startTime = time / _distributionTime;
        var endTime = (time + deltaTime) / _distributionTime;

        var startValue = _distributionCurve.Evaluate(startTime);
        var endValue = _distributionCurve.Evaluate(endTime);
        var averageValue = (startValue + endValue) * 0.5f;

        var value = averageValue * _curveNormalization * deltaTime;
        return value;
    }
}