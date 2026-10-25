using UnityEngine;

public interface IHitmark
{
    public Vector2 Size { get; }

    public void SetSize(Vector2 size);
    public void SetDepth(float depth);
    public void SetPivot(Vector3 pivot);
}
