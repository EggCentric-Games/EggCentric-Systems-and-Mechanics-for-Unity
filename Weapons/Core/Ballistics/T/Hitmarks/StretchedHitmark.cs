using UnityEngine;

namespace EggCentric.Ballistics
{
    [System.Serializable]
    public class StretchedHitmark : HitmarkerSpawner
    {
        protected override float GetMarkDepth(Vector3 normal, float _) => 1f;

        protected override Vector3 GetMarkFacingDirection(Vector3 normal) => -normal;

        protected override Vector3 GetMarkPivot(float depth) => Vector3.forward * depth;

        protected override Vector2 GetMarkSize()
        {
            throw new System.NotImplementedException();
        }
    }
}