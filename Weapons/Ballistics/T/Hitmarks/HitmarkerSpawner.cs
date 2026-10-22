using UnityEngine;

namespace EggCentric.Ballistics
{
    [System.Serializable]
    public abstract class HitmarkerSpawner
    {
        [SerializeField] protected Transform reference;
        [SerializeField] protected float hitmarkOffset;
        [SerializeField] protected Hitmark hitmarkPrefab;

        public void SpawnHitmark(Transform parent, Vector3 position, Vector3 normal, float sizeMultiplier = 1f)
        {
            float decalDepth = GetMarkDepth(normal, sizeMultiplier);
            Vector2 decalSize = GetMarkSize() * sizeMultiplier;

            Vector3 facingDirection = GetMarkFacingDirection(normal);
            var randomRotation = Quaternion.AngleAxis(Random.Range(0f, 360f), facingDirection);
            Vector3 pivot = GetMarkPivot(decalDepth);
            var hitmark = GameObject.Instantiate(hitmarkPrefab, position, Quaternion.FromToRotation(Vector3.forward, facingDirection) * randomRotation, parent);
            hitmark.SetSize(decalSize);
            hitmark.SetDepth(decalDepth);
            hitmark.SetPivot(pivot);
        }

        protected abstract Vector3 GetMarkFacingDirection(Vector3 normal);
        protected abstract Vector2 GetMarkSize();
        protected abstract float GetMarkDepth(Vector3 normal, float sizeMultiplier = 1f);
        protected abstract Vector3 GetMarkPivot(float depth);

    }
}