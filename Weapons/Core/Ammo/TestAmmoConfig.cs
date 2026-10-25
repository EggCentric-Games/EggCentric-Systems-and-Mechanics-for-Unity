using EggCentric.Ballistics;
using UnityEngine;

[CreateAssetMenu(fileName = "AmmoConfig_", menuName = "EggCentric/Weapons/Configs/Ammo")]
public class TestAmmoConfig : ScriptableObject
{
    public Projectile ProjectilePrefab => _projectilePrefab;
    public int ProjectileCount => _projectileCount;

    [SerializeField] private Projectile _projectilePrefab;
    [SerializeField] [Min(0)] private int _projectileCount;
}
