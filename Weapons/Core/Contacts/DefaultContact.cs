namespace EggCentric.Ballistics
{
    public struct DefaultContact : IContact
    {
        public ProjectileData Projectile { get; }
        public HitData Hit { get; }

        public DefaultContact(ProjectileData projectileData, HitData hitData)
        {
            Projectile = projectileData;
            Hit = hitData;
        }
    }
}