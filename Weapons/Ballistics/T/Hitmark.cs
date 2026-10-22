using UnityEngine;
using UnityEngine.Rendering.Universal;

public interface IHitmark
{
    public Vector2 Size { get; }

    public void SetSize(Vector2 size);
    public void SetDepth(float depth);
    public void SetPivot(Vector3 pivot);
}

public class Hitmark : MonoBehaviour, IHitmark
{
    [SerializeField] private DecalProjector projector;

    public Vector2 Size => projector.size;

    public void SetSize(Vector2 size)
    {
        Vector3 targetSize = size;
        targetSize.z = projector.size.z;

        projector.size = targetSize;
    }

    public void SetDepth(float depth)
    {
        Vector3 targetSize = projector.size;
        targetSize.z = depth;

        projector.size = targetSize;
    }

    public void SetPivot(Vector3 pivot)
    {
        projector.pivot = pivot;
    }
}
