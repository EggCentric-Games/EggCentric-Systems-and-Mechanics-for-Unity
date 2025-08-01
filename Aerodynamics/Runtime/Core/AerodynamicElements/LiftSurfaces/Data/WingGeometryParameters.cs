using UnityEngine;


namespace EggCentric.Aerodynamics
{
    [System.Serializable]
    public struct WingGeometryParameters
    {
        public float SurfaceArea => AverageChord * Span * _chordCorrection;
        public float AverageChord => (RootWidth + TipWidth) * 0.5f;
        private float _chordCorrection => GetChordCorrection();

        public bool IsMirrored;
        [Range(-1, 1)]
        public float Verticality;
        [Min(0)]
        public float Span;
        [Min(0)]
        public float RootWidth;
        [Min(0)]
        public float TipWidth;
        public float TipOffset;

        private float GetChordCorrection()
        {
            Vector3 sweepDirection = ((Vector3.forward * TipOffset) + (Vector3.right * Span)).normalized;
            float chordCorrection = Vector3.Dot(Vector3.right, sweepDirection);

            return chordCorrection;
        }
    }
}