using UnityEngine;

namespace EggCentric.Aerodynamics
{
    public class ControlSurface : LiftSurface
    {
        [SerializeField] private Vector2 _angleLimits;

        private Quaternion _rotation;

        public void SetDeflection(float deflectionInput)
        {
            float deflectionAngle = GetDeflectionAngle(deflectionInput);
            _rotation = Quaternion.AngleAxis(deflectionAngle, surfaceAxis);

            Debug.Log($"{name}: {deflectionInput} - {deflectionAngle}");
            transform.localRotation = _rotation;
        }

        protected override Vector3 GetSurfaceChord()
        {
            Vector3 baseChord = base.GetSurfaceChord();

            return _rotation * baseChord;
        }

        protected override Vector3 GetSurfaceNormal()
        {
            Vector3 baseNormal = base.GetSurfaceNormal();

            return _rotation * baseNormal;
        }

        private float GetDeflectionAngle(float deflectionInput)
        {
            deflectionInput = Mathf.Clamp(deflectionInput, -1.0f, 1.0f);

            float deflectionMagnitude = Mathf.Abs(deflectionInput);
            float deflectionDirection = Mathf.Sign(deflectionInput);

            float maxAngle = deflectionDirection >= 0 ? _angleLimits.y : _angleLimits.x;
            float deflectionAngle = Mathf.Lerp(0, maxAngle, deflectionMagnitude);

            return deflectionAngle;
        }
    }
}