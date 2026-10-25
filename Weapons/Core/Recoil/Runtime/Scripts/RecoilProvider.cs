using UnityEngine;

public abstract class RecoilProvider : MonoBehaviour
{
    [SerializeField] protected Rigidbody _recoilBody;

    public abstract void ApplyRecoil();
}
