using UnityEngine;

namespace EggCentric.Aerodynamics
{
    public class LiftSurface : AbstractLiftSurface
    {
        public override float SurfaceArea => _wingGeometryParameters.SurfaceArea;
        public override float Span => _wingGeometryParameters.Span;

        protected Vector3 surfaceAxis => Vector3.Slerp(Vector3.right, Vector3.up * Mathf.Sign(_wingGeometryParameters.Verticality) * (_wingGeometryParameters.IsMirrored ? -1f : 1f), Mathf.Abs(_wingGeometryParameters.Verticality));
        protected Vector3 surfaceNormal => GetSurfaceNormal();
        protected Vector3 surfaceChord => GetSurfaceChord();

        [SerializeField] private AerodynamicConfig aerodynamicConfig;
        [SerializeField] private WingGeometryParameters _wingGeometryParameters;
        [SerializeField] private AerodynamicParameters _aerodynamicParameters;

        [Header("Debug")]
        [SerializeField] private bool _showBounds = true;
        [SerializeField] private Color _drawColor = Color.blue;

        private IAerodynamicCoefficientCalculator _aerodynamicCalculator;

        protected override AerodynamicForces GetAerodynamicForces(Rigidbody aircraft)
        {
            Vector3 localAirflowVelocity = GetLocalAirflowVelocity(aircraft);
            Debug.DrawLine(transform.position, transform.position + transform.TransformDirection(localAirflowVelocity), new Color(1f, 1f, 0f));
            //transform.InverseTransformDirection(worldAirflowVelocity);

            float AoA = GetAoA(localAirflowVelocity);

            float aspectRatio = ParentSurface ? ParentSurface.AspectRatio : AspectRatio;
            AerodynamicCoefficients aerodynamicCoefficients = _aerodynamicCalculator.GetAerodynamicCoefficients(_aerodynamicParameters, AoA, aspectRatio);
            Debug.Log($"Surface: {name}; AoA: {AoA}; Coeficients: {aerodynamicCoefficients.Drag}; {aerodynamicCoefficients.Lift}; {aerodynamicCoefficients.Torque}");

            float airDensity = 1f;
            float dynamicPressure = airDensity * localAirflowVelocity.sqrMagnitude * 0.5f;
            Debug.Log($"Surface: {name}; Pressure: {dynamicPressure}; Area: {SurfaceArea}");

            Vector3 dragDirection = transform.TransformDirection(localAirflowVelocity.normalized);
            Vector3 liftDirection = transform.TransformDirection(Vector3.Cross(surfaceAxis, localAirflowVelocity.normalized).normalized);
            Debug.Log($"Surface: {this}; Normal: {surfaceNormal}; Lift: {liftDirection}");

            AerodynamicForces aerodynamicForces = new AerodynamicForces();
            aerodynamicForces.Drag = dragDirection * aerodynamicCoefficients.Drag * dynamicPressure * SurfaceArea;
            aerodynamicForces.Lift = liftDirection * aerodynamicCoefficients.Lift * dynamicPressure * SurfaceArea;

            Vector3 torqueByForce = Vector3.Cross(transform.position - aircraft.worldCenterOfMass, aerodynamicForces.Drag + aerodynamicForces.Lift);
            Vector3 pitchingMoment = transform.TransformDirection(-surfaceAxis) * aerodynamicCoefficients.Torque * dynamicPressure * SurfaceArea * _wingGeometryParameters.Span;
            aerodynamicForces.Torque = torqueByForce + pitchingMoment;

            Debug.Log($"CD: {aerodynamicCoefficients.Drag}; CL: {aerodynamicCoefficients.Lift}; Nav Speed: {aircraft.velocity.magnitude}");
            return aerodynamicForces;
        }

        protected virtual Vector3 GetSurfaceNormal()
        {
            return Vector3.Cross(surfaceAxis, Vector3.forward).normalized;
        }

        protected virtual Vector3 GetSurfaceChord()
        {
            return Vector3.forward;
        }

        protected override void Awake()
        {
            base.Awake();
            _aerodynamicCalculator = new AerodynamicCoefficientCalculator();

            for (int i = -180; i <= 180; i++)
            {
                var coefficients = _aerodynamicCalculator.GetAerodynamicCoefficients(_aerodynamicParameters, i, AspectRatio);
                Debug.Log($"Surface: {this}; Aspect Ratio: {AspectRatio}\nAoA: {i}; Coefficients: {coefficients.Lift}; {coefficients.Drag}; {coefficients.Torque}");
            }
        }

        private Vector3 GetLocalAirflowVelocity(Rigidbody airplane)
        {
            Vector3 worldAirflowVelocity = -airplane.GetPointVelocity(transform.position);
            Vector3 localAirflowVelocity = transform.InverseTransformDirection(worldAirflowVelocity);
            Debug.DrawLine(transform.position, transform.position + localAirflowVelocity, Color.green);
            localAirflowVelocity = Vector3.ProjectOnPlane(localAirflowVelocity, surfaceAxis);

            return localAirflowVelocity;
        }

        private float GetAoA(Vector3 localVelocity)
        {
            return Vector3.SignedAngle(surfaceChord, -localVelocity, surfaceAxis);
        }

        private Mesh GetWingMesh()
        {
            int mirrorModifier = _wingGeometryParameters.IsMirrored ? -1 : 1;

            Vector3 spanOffset = surfaceAxis * _wingGeometryParameters.Span * mirrorModifier * 0.5f;
            Vector3 rootWidthOffset = GetSurfaceChord() * _wingGeometryParameters.RootWidth * 0.5f;
            Vector3 tipWidthOffset = GetSurfaceChord() * _wingGeometryParameters.TipWidth * 0.5f;
            Vector3 additionalTipOffset = GetSurfaceChord() * _wingGeometryParameters.TipOffset;

            Vector3[] vertices = new Vector3[4];
            vertices[0] = -spanOffset - rootWidthOffset;
            vertices[1] = -spanOffset + rootWidthOffset;
            vertices[2] = spanOffset - tipWidthOffset + additionalTipOffset;
            vertices[3] = spanOffset + tipWidthOffset + additionalTipOffset;

            int[] triangles = new int[2 * 3 * 2];
            triangles[0] = 0;
            triangles[1] = 1;
            triangles[2] = 2;
            triangles[3] = 1;
            triangles[4] = 3;
            triangles[5] = 2;
            //backfaces
            triangles[6] = 0;
            triangles[7] = 2;
            triangles[8] = 1;
            triangles[9] = 1;
            triangles[10] = 2;
            triangles[11] = 3;

            Mesh mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();

            return mesh;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = _drawColor;
            Gizmos.DrawMesh(GetWingMesh(), transform.position, transform.rotation);
        }
    }
}