using UnityEngine;

namespace EggCentric.Stabbing
{
    public struct BodyHit
    {
        private RaycastHit2D contact;
        private StabbableBody body;
        private Blade blade;

        public RaycastHit2D Contact => contact;
        public StabbableBody Body => body;
        public Blade Blade => blade;

        public BodyHit(RaycastHit2D contact, StabbableBody body, Blade blade)
        {
            this.contact = contact;
            this.body = body;
            this.blade = blade;
        }
    }
}