using System;
using UnityEngine;

public static class Utilities
{
    public static SliderJoint2D CreateSliderJoint(SliderJointConfig config)
    {
        SliderJoint2D joint = config.MainBody.gameObject.AddComponent<SliderJoint2D>();
        joint.connectedBody = config.ConnectedBody;
        joint.anchor = config.Anchor;
        joint.connectedAnchor = config.ConnectedAnchor;
        joint.enableCollision = config.EnableCollision;
        joint.autoConfigureConnectedAnchor = config.AutoAnchor;

        joint.autoConfigureAngle = config.AutoConfigureAngle;
        joint.angle = config.Angle;
        joint.useMotor = config.UseMotor;
        joint.breakAction = config.JointBreakAction;
        joint.motor = config.Motor;

        return joint;
    }

    public static FrictionJoint2D CreateFrictionJoint(FrictionJointConfig config)
    {
        FrictionJoint2D joint = config.MainBody.gameObject.AddComponent<FrictionJoint2D>();
        joint.connectedBody = config.ConnectedBody;
        joint.enableCollision = config.EnableCollision;
        joint.autoConfigureConnectedAnchor = config.AutoAnchor;
        joint.anchor = config.Anchor;
        joint.connectedAnchor = config.ConnectedAnchor;
        joint.breakAction = config.JointBreakAction;

        joint.maxForce = config.ForceDrag;
        joint.maxTorque = config.TorqueDrag;

        return joint;
    }

    [Obsolete("Collider2D.ClosestPoint should do the same")]
    public static Vector2 PointToBodyProjection(Vector2 point, Collider2D targetCollider)
    {
        if (targetCollider.OverlapPoint(point))
            return point;

        return targetCollider.ClosestPoint(point);
    }
}