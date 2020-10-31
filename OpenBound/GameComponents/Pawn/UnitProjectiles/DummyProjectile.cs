using Microsoft.Xna.Framework;
using OpenBound.Common;
using OpenBound.GameComponents.Animation;
using OpenBound.GameComponents.MobileAction;
using OpenBound_Network_Object_Library.Entity;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace OpenBound.GameComponents.Pawn.UnitProjectiles
{
    public class DummyProjectile : Projectile
    {
        public DummyProjectile(Mobile mobile, ShotType shotType, int explosionRadius, int baseDamage, bool canCollide = true)
            : base(mobile, shotType, explosionRadius, baseDamage, canCollide: canCollide)
        {
            this.Mobile = mobile;

            //Initializing Flipbook
            FlipbookList.Add(new Flipbook(
                mobile.Crosshair.CannonPosition, Vector2.One / 2,
                1, 1, "Misc/Dummy",
                new List<AnimationInstance>() {
                    new AnimationInstance()
                }, DepthParameter.Projectile, angle));
        }

        protected override void Destroy()
        {
            base.Destroy();

            List<Projectile> pjList = Mobile.ProjectileList.Except(Mobile.UnusedProjectile).ToList();

            if (pjList.Count() == 0)
                OnFinalizeExecutionAction?.Invoke();
        }
    }
}
