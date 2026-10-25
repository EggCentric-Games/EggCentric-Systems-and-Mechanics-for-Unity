using UnityEngine;

namespace EggCentric.Ballistics
{
    [System.Serializable]
    public class AngleBasedHitmark : HitmarkerSpawner
    {
        protected override float GetMarkDepth(Vector3 normal, float sizeMultiplier)
        {
            float hitAngle = Vector3.Angle(-reference.forward, normal);
            float tangent = Mathf.Tan(hitAngle * Mathf.Deg2Rad);

            float diagonalExtents = hitmarkPrefab.Size.magnitude * sizeMultiplier;
            return diagonalExtents * tangent;
        }

        protected override Vector3 GetMarkFacingDirection(Vector3 normal) => reference.forward;
        protected override Vector3 GetMarkPivot(float _) => Vector3.zero;
        protected override Vector2 GetMarkSize() => hitmarkPrefab.Size;
    }
}