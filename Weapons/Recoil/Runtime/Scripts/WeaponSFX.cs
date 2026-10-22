using EggCentric.Sights;
using FMODUnity;
using UnityEngine;

public class WeaponSFX : SFXProvider
{
    [SerializeField] private EventReference _event;

    public override void PlayShot()
    {
        RuntimeManager.PlayOneShot(_event, transform.position);
    }
}