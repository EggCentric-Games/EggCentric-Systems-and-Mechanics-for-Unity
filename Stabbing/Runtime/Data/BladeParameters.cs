using UnityEngine;

namespace EggCentric.Stabbing
{
    [System.Serializable]
    public struct BladeParameters
    {
        public Vector2 EdgeDirection => edgeDirection;
        public float LowerAngleLimit => lowerAngleLimit;
        public float UpperAngleLimit => upperAngleLimit;
        public bool IsBidirectional => isBidirectional;
        public bool IsVariableDirection => isVariableDirection;

        [SerializeField] private Vector2 edgeDirection;
        [SerializeField] private float lowerAngleLimit;
        [SerializeField] private float upperAngleLimit;
        [SerializeField] private bool isBidirectional;
        [SerializeField] private bool isVariableDirection;
    }
}