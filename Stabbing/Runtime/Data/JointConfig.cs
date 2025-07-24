using UnityEngine;

public class JointConfig
{
    public Rigidbody2D MainBody;
    public Rigidbody2D ConnectedBody;
    internal bool AutoAnchor = true;
    public Vector2 Anchor = Vector2.one * 0.5f;
    public Vector2 ConnectedAnchor = Vector2.one * 0.5f;
    public bool EnableCollision = false;
    public JointBreakAction2D JointBreakAction = JointBreakAction2D.Destroy;

    public JointConfig(Rigidbody2D mainBody, Rigidbody2D connectedBody)
    {
        MainBody = mainBody;
        ConnectedBody = connectedBody;
    }

    public JointConfig(Rigidbody2D mainBody, Rigidbody2D connectedBody, Vector2 anchorPoint) : this(mainBody, connectedBody)
    {
        Anchor = mainBody.transform.InverseTransformPoint(anchorPoint);
        ConnectedAnchor = connectedBody.transform.InverseTransformPoint(anchorPoint);
    }
}
