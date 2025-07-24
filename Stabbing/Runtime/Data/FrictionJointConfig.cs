using UnityEngine;

public class FrictionJointConfig : JointConfig
{
    public float ForceDrag = 0f;
    public float TorqueDrag = 0f;

    public FrictionJointConfig(Rigidbody2D mainBody, Rigidbody2D connectedBody, Vector2 anchorPoint) : base(mainBody, connectedBody, anchorPoint)
    {
    }
}