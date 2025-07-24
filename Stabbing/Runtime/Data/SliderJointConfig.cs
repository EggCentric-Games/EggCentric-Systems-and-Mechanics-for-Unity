using UnityEngine;

public class SliderJointConfig : JointConfig
{
    public bool AutoConfigureAngle = true;
    public float Angle = 0f;
    public bool UseMotor = false;
    public JointMotor2D Motor = new JointMotor2D();

    public SliderJointConfig(Rigidbody2D mainBody, Rigidbody2D connectedBody, Vector2 anchorPoint, Vector2 slideDirection) : base(mainBody, connectedBody, anchorPoint)
    {
        Angle = Vector3.SignedAngle(mainBody.transform.up, slideDirection, Vector3.forward) - 90f;
    }
}
