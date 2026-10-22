using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class ProjectileSettings
{
    public float FlightDistance => _flightDistance;
    public float DeviationMultiplier => _deviationMultiplier;
    public LayerMask LayerMask => _layerMask;
    public QueryTriggerInteraction TriggerInteraction => _triggerInteraction;
    public float Epsilon => _epsilon;

    [SerializeField] private float _flightDistance;
    [SerializeField] private float _deviationMultiplier;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private QueryTriggerInteraction _triggerInteraction;
    [SerializeField] private float _epsilon = 0.001f;
}

public class Penetrator
{
    private ProjectileSettings _settings;

    private bool _isSubmerged => _penetratedBodies > 0;
    private FlightPath _path;
    private Vector3 _flightDirection;
    private Vector3 _bulletPosition;
    private int _penetratedBodies;
    private IOngoingSegment _currentSegment;

    public Penetrator(ProjectileSettings projectileSettings, Vector3 startPos, Vector3 flightDirection)
    {
        _settings = projectileSettings;

        _path = new FlightPath();
        _bulletPosition = startPos;
        _flightDirection = flightDirection;

        _currentSegment = PathSegment.Start(_bulletPosition);
        CheckForLength(_settings.FlightDistance);
    }

    public void Draw()
    {
        _path.Draw();
    }

    private void CheckForLength(float length)
    {
        float remainingLength = length;

        while (remainingLength > 0)
        {
            var hasHit = ForwardCheck(_bulletPosition, _settings.FlightDistance, out var hitInfo);
            var traveledDistance = hasHit ? hitInfo.distance : remainingLength;
            traveledDistance += _settings.Epsilon;

            var endPoint = _bulletPosition + _flightDirection * traveledDistance;

            if (!_isSubmerged)
            {
                if(!hasHit)
                    CloseSegment(endPoint, _isSubmerged);
                else
                    HandleEnter(endPoint, hitInfo.normal);

                MoveTo(endPoint);
                remainingLength -= traveledDistance;
                continue;
            }

            var hasExited = BackCheck(endPoint, traveledDistance, out var hit);
            if (!hasExited)
            {
                if (!hasHit)
                    CloseSegment(endPoint, _isSubmerged);
                else
                    HandleEnter(endPoint, hit.normal);

                MoveTo(endPoint);
                remainingLength -= traveledDistance;
                continue;
            }

            endPoint = hit.point + (_flightDirection * _settings.Epsilon);
            traveledDistance -= hit.distance;

            HandleExit(endPoint, hit.normal);
            MoveTo(endPoint);
            remainingLength -= traveledDistance;
            continue;
        }
    }

    private void HandleEnter(Vector3 point, Vector3 normal)
    {
        CloseSegment(point, _isSubmerged);
        _flightDirection = GetDeviatedDirection(_flightDirection, normal);
        _penetratedBodies++;
    }

    private void HandleExit(Vector3 point, Vector3 normal)
    {
        CloseSegment(point, _isSubmerged);
        //_flightDirection = GetDeviatedDirection(_flightDirection, normal);
        _penetratedBodies--;
    }

    private void CloseSegment(Vector3 endPoint, bool throughVolume)
    {
        var finishedSegment = _currentSegment.Finish(endPoint, throughVolume);
        _path.Add(finishedSegment);
        _currentSegment = PathSegment.Start(endPoint);
    }

    private Vector3 GetDeviatedDirection(Vector3 flightDirection, Vector3 normal)
    {
        var dot = Vector3.Dot(flightDirection, normal);
        var grazing = 1 - Mathf.Abs(dot);
        var deviationMagnitude = grazing * _settings.DeviationMultiplier;

        var impactAngle = 90f - Vector3.Angle(flightDirection, normal);
        var outputAngle = Mathf.Lerp(impactAngle, impactAngle * 0.5f, deviationMagnitude);
        var delta = outputAngle - impactAngle;

        Debug.Log($"Impact: {impactAngle}, Output: {outputAngle}, Delta: {delta}");

        var deviationAxis = Vector3.Cross(flightDirection, normal);
        return Quaternion.AngleAxis(delta, deviationAxis.normalized) * flightDirection;

        //Vector3 reflected = Vector3.Reflect(flightDirection, normal);
        //return Vector3.Lerp(flightDirection, reflected, _deviationMultiplier);
    }

    private Vector3 GetDeviatedDirectionGPT(Vector3 flightDirection, Vector3 normal)
    {
        Vector3 n = normal.normalized;
        Vector3 d = flightDirection.normalized;

        float cosIn = Vector3.Dot(-d, n); // incidence
        Vector3 tangent = (d + cosIn * n).normalized; // projected direction

        float grazing = 1f - Mathf.Abs(Vector3.Dot(d, n));  // 0 = head-on, 1 = grazing
        float deflect = grazing * _settings.DeviationMultiplier;

        // Straight-through direction (bullet wants to go forward)
        Vector3 straightThrough = d;

        // Deflected direction (bent along surface)
        Vector3 bent = (tangent * (1f - deflect) - n * cosIn).normalized;

        // Blend
        Vector3 outDir = Vector3.Slerp(straightThrough, bent, deflect);
        return outDir.normalized;

        //Vector3 reflected = Vector3.Reflect(flightDirection, normal);
        //return Vector3.Lerp(flightDirection, reflected, _deviationMultiplier);
    }

    private bool ForwardCheck(Vector3 origin, float distance, out RaycastHit hitInfo)
    {
        return Physics.Raycast(origin, _flightDirection, out hitInfo, distance, _settings.LayerMask, _settings.TriggerInteraction);
    }

    private bool BackCheck(Vector3 origin, float distance, out RaycastHit hitInfo)
    {
        hitInfo = default;
        var hits = Physics.RaycastAll(origin, -_flightDirection, distance, _settings.LayerMask, _settings.TriggerInteraction);
        
        if(hits.Length <= 0)
            return false;

        hitInfo = hits.OrderByDescending(x => x.distance).First();
        return true;
    }

    private void MoveTo(Vector3 position) => _bulletPosition = position;
}

public class FlightPath
{
    public IEnumerable<IFinishedSegment> Segments => _segments;

    private readonly List<IFinishedSegment> _segments;

    public FlightPath()
    {
        _segments = new List<IFinishedSegment>();
    }

    public void Add(IFinishedSegment segment)
    {
        _segments.Add(segment);
    }

    public void Draw()
    {
        for (int i = 0; i < _segments.Count; i++)
        {
            var segment = _segments[i];
            segment.Draw();
        }
    }
}

public interface IPathSegment
{
    public Vector3 StartPoint { get; }
    public Vector3 EndPoint { get; }
}

public interface IOngoingSegment : IPathSegment
{
    public IFinishedSegment Finish(Vector3 point, bool throughVolume = true);
}

public interface IFinishedSegment : IPathSegment
{
    public void Draw();
}

public class PathSegment : IOngoingSegment, IFinishedSegment
{
    public Vector3 StartPoint { get; }
    public Vector3 EndPoint { get; private set; }

    private bool _throughVolume;

    private PathSegment(Vector3 startPoint)
    {
        StartPoint = startPoint;
    }

    public static IOngoingSegment Start(Vector3 point)
    {
        return new PathSegment(point);
    }

    public IFinishedSegment Finish(Vector3 point, bool throughVolume = true)
    {
        EndPoint = point;
        _throughVolume = throughVolume;
        return this;
    }

    public void Draw()
    {
        var color =  _throughVolume ? Color.red : Color.green;
        Gizmos.color = color;
        Gizmos.DrawWireSphere(StartPoint, 0.01f);
        Gizmos.DrawWireSphere(EndPoint, 0.01f);
        Debug.DrawLine(StartPoint, EndPoint, color);
    }
}