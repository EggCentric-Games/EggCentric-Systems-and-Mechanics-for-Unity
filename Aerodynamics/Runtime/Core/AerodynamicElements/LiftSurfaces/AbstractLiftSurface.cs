using EggCentric.ValueProviders.DataContainers;
using EggCentric.QoL;
using UnityEngine;


namespace EggCentric.Aerodynamics
{
    public abstract class AbstractLiftSurface : MonoBehaviour, IAerodynamicElement
    {
        public AerodynamicForces AerodynamicForces => _forceCache;
        public float AspectRatio => Span * Span / SurfaceArea;
        public bool IsCompositeElement => ParentSurface;

        protected CompositeLiftSurface ParentSurface { get; private set; }
        protected Rigidbody AttachedRigidbody => ParentSurface ? ParentSurface.AttachedRigidbody : _attachedRigidbody;

        public abstract float SurfaceArea { get; }
        public abstract float Span { get; }

        private Rigidbody _attachedRigidbody;
        private AutomatedDataCache<AerodynamicForces> _forceCache;

        protected virtual void Awake()
        {
            InitializeSelf();
        }

        private void InitializeSelf()
        {
            ResolveParent();
            ResolveAttachedBody();

            Debug.Log($"Surface: {this}; Parent: {ParentSurface}; Rigidbody: {AttachedRigidbody}");

            _forceCache = new AutomatedDataCache<AerodynamicForces>(new TimeDependentDataCache<AerodynamicForces>(), () => GetAerodynamicForces(AttachedRigidbody));
        }

        private void ResolveParent()
        {
            ParentSurface = TryToFindParent();
        }

        private void ResolveAttachedBody()
        {
            transform.TryGetComponentInParent(out _attachedRigidbody);
        }

        public void AssignParent(CompositeLiftSurface parent)
        {
            if (parent == this)
            {
                Debug.LogWarning($"Lift surface can not be parent of itself! Assign attempt will be ignored.");
                return;
            }

            ParentSurface = parent;
        }

        private void OnEnable()
        {
            ParentSurface = TryToFindParent();
        }

        private void OnValidate()
        {
            ParentSurface = TryToFindParent();
        }

        private CompositeLiftSurface TryToFindParent()
        {
            if (ParentSurface)
                return ParentSurface;

            if (this.TryGetComponentInParent(out CompositeLiftSurface parent))
                parent.AddElement(this);

            return parent;
        }

        private void DetachFromParent()
        {
            if (!ParentSurface)
                return;

            ParentSurface.RemoveElement(this);
            ParentSurface = null;
        }

        private void OnDisable()
        {
            DetachFromParent();
        }

        private void OnDestroy()
        {
            DetachFromParent();
        }

        protected abstract AerodynamicForces GetAerodynamicForces(Rigidbody aircraft);
    }
}