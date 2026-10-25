using UnityEngine;

namespace EggCentric.Ballistics
{
    [System.Serializable]
    public class SurfaceNormalHitmark : HitmarkerSpawner
    {
        protected override float GetMarkDepth(Vector3 normal, float _) => hitmarkOffset;

        protected override Vector3 GetMarkFacingDirection(Vector3 normal) => -normal;

        protected override Vector3 GetMarkPivot(float depth) => Vector3.zero;

        protected override Vector2 GetMarkSize() => hitmarkPrefab.Size;
    }
}