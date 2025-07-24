using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class StabbableBody : MonoBehaviour
{
    [SerializeField] private Collider2D bodyCollider;
    [SerializeField] private float density;

    public Collider2D Collider => bodyCollider;
    public float Density => density;

    private void OnValidate()
    {
        if(!bodyCollider)
            bodyCollider = GetComponent<Collider2D>();
    }
}
