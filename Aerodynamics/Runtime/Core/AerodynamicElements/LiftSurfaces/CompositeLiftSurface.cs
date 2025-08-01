using System.Collections.Generic;
using UnityEngine;


namespace EggCentric.Aerodynamics
{
    public class CompositeLiftSurface : AbstractLiftSurface
    {
        public override float SurfaceArea => GetTotalArea();
        public override float Span => GetTotalSpan();

        private HashSet<AbstractLiftSurface> surfaceElements = new HashSet<AbstractLiftSurface>();

        public void AddElement(AbstractLiftSurface newElement)
        {
            surfaceElements.Add(newElement);
        }

        public void RemoveElement(AbstractLiftSurface element)
        {
            surfaceElements.Remove(element);
        }

        protected override AerodynamicForces GetAerodynamicForces(Rigidbody aircraft)
        {
            AerodynamicForces totalForces = default;
            foreach (var element in surfaceElements)
            {
                totalForces.Drag += element.AerodynamicForces.Drag;
                totalForces.Lift += element.AerodynamicForces.Lift;
                Debug.Log($"Surface: {element}; Torque: {element.AerodynamicForces.Torque}");
                totalForces.Torque += element.AerodynamicForces.Torque;
            }

            return totalForces;
        }

        private float GetTotalArea()
        {
            float totalArea = 0;
            foreach (var element in surfaceElements)
            {
                totalArea += element.SurfaceArea;
            }

            return totalArea;
        }

        private float GetTotalSpan()
        {
            float totalSpan = 0;
            foreach (var element in surfaceElements)
            {
                totalSpan += element.Span;
            }

            return totalSpan;
        }
    }
}