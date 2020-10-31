using Microsoft.Xna.Framework;
using OpenBound.Common;
using OpenBound.GameComponents.Debug;
using OpenBound.GameComponents.Level;
using OpenBound.GameComponents.Level.Scene;
using OpenBound.GameComponents.MobileAction;
using OpenBound.GameComponents.WeatherEffect;
using OpenBound_Network_Object_Library.Entity;
using OpenBound_Network_Object_Library.Entity.Sync;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenBound.GameComponents.Pawn.UnitProjectiles
{
    public enum BeamEmitterType
    {
        Lightning,
        Weather,
        Thor,
    }

    /// <summary>
    /// Creates a beam dummy projectile. This projectile has its own movement rules being updated all at once.
    /// </summary>
    public class BeamDummyProjectile : DummyProjectile
    {
        //Stores all mobiles affected by the proximity with the lightning
        HashSet<Mobile> mobileList;
        List<Mobile> completeMobileList;

        //Lightning effect variables
        int extraExplosionRadius;
        int extraDamage;
        protected float beamAngle;
        
        //Position of parent projectile
        Vector2 parentPosition;
        Vector2 positionOffset;

        //Thor exclusive variable
        Vector2 finalPosition;

        //Behavior variables
        BeamEmitterType beamEmitterType;

        public BeamDummyProjectile(Mobile mobile, Vector2 parentPosition, float beamAngle, int explosionRadius, int extraExplosionRadius, int baseDamage, int extraDamage, BeamEmitterType beamEmitterType, Vector2? finalPosition = null)
            : base(mobile, ShotType.Dummy, explosionRadius, baseDamage)
        {
            this.extraExplosionRadius = extraExplosionRadius;
            this.extraDamage = extraDamage;
            this.beamAngle = beamAngle;
            this.parentPosition = parentPosition;
            this.beamEmitterType = beamEmitterType;

            if (finalPosition != null)
                this.finalPosition = (Vector2)finalPosition;

            positionOffset = Vector2.Transform(Vector2.UnitX * 3, Matrix.CreateRotationZ(-beamAngle));
            mobileList = new HashSet<Mobile>();
        }

        /// <summary>
        /// Executes all possible movements at once. Starting at <see cref="parentPosition"/> and finishing at the first pixel outside the area.
        /// </summary>
        public override void Update()
        {
            //This method starts at parentPosition and keep adding a offset until the position is outside the map boundaries.
            //While it hasn't found any collidable spot it keeps saving all mobiles that are going to be affected by the discharge extra effect
            //If it finds another collidable spot, resets all potential affected mobile list and save this position

            //Starts at parent position
            Vector2 explosionPosition = Position = parentPosition;

            //Weather electricity effect has different behavior than lightning's
            
            //The weather needs to have a top-bottom collision checking approach. On the other hand,
            //the Lightning projectile collision starts at the project itself and then checks farther positions
            //because of its rotating characteristic
            switch (beamEmitterType)
            {
                /*case BeamEmitterType.Weather:
                    WeatherUpdateBehavior(ref explosionPosition);
                    break;*/
                case BeamEmitterType.Lightning:
                    LightningUpdateBehavior(ref explosionPosition);
                    break;
                case BeamEmitterType.Thor:
                    ThorUpdateBehavior(ref explosionPosition);
                    break;
            }

            //In the end, move the projectile to the last "explodable" spot and explode it
            Position = explosionPosition;
            Explode();
        }

        private void LightningUpdateBehavior(ref Vector2 explosionPosition)
        {
            completeMobileList = LevelScene.DamageableMobiles.ToList();

            //While it is still inside the map
            while (Topography.IsInsideMapBoundaries(Position) && Position.Y >= Topography.FirstCollidableBlockY)
            {
                //Check if there is any mobile whithin the extraExplosionRadius to receive the electrical extra dmaage
                CheckAffectedMobiles();

                //If it collides with the ground or with another mobile, reset all previous affected mobiles and save the new explosion position
                if (Topography.CheckCollision(Position) || LevelScene.MobileList.Any((m) => m.CollisionBox.CheckCollision(Position)))
                {
                    mobileList.Clear();
                    explosionPosition = Position;
                }

                Position += positionOffset;
            }
        }

        private void ThorUpdateBehavior(ref Vector2 explosionPosition)
        {
            completeMobileList = LevelScene.MobileList.ToList();

            //While it is still inside the map
            while (
                Topography.IsInsideMapBoundaries(Position) &&
                Position.Y >= Topography.FirstCollidableBlockY &&
                Helper.SquaredEuclideanDistance(finalPosition, Position) > Parameter.ProjectileThorBeamDistanceThreshold)
            {
                //Check if there is any mobile whithin the extraExplosionRadius to receive the electrical extra dmaage
                CheckAffectedMobiles();

                //If it collides with the ground or with another mobile, reset all previous affected mobiles and save the new explosion position
                if (Topography.CheckCollision(Position) || LevelScene.MobileList.Any((m) => m.CollisionBox.CheckCollision(Position)))
                {
                    mobileList.Clear();
                    explosionPosition = Position;
                }

                Position += positionOffset;
            }
        }

        /*private void WeatherUpdateBehavior(ref Vector2 explosionPosition)
        {
            completeMobileList = LevelScene.DamageableMobiles.Where((x) => Math.Abs(x.Position.Y) - Math.Abs(Position.Y) <= extraExplosionRadius).ToList();

            //Start at the first (possible) collidable position
            Position = new Vector2(Position.X, Topography.FirstCollidableBlockY);

            //Find the first possible exploding position
            while (Topography.IsInsideMapBoundaries(Position))
            {
                CheckAffectedMobiles();

                //If it collides with the ground, stop right there
                if (Topography.CheckCollision(Position))
                {
                    explosionPosition = Position;
                    break;
                }

                Position -= positionOffset;
            }

            if (explosionPosition == parentPosition)
            {
                explosionPosition = new Vector2(Position.X, Topography.MapHeight / 2);
            }
        }*/

        /// <summary>
        /// Checks if every mobile can be affected by the extra discharge damage
        /// </summary>
        public void CheckAffectedMobiles()
        {
            foreach (Mobile m in completeMobileList)
            {
                double distance = m.CollisionBox.GetSquaredDistance(FlipbookList[0].Position, ExplosionRadius);

                if (distance < extraExplosionRadius * extraExplosionRadius)
                {
                    mobileList.Add(m);
                }
            }
        }

        public override void Explode()
        {
            base.Explode();

            //Deals damage to any mobile whithin the extra damage area
            foreach (Mobile m in mobileList)
            {
                float previousHealth = m.MobileMetadata.CurrentHealth;
                bool wasTargetKilled = false;
                m.ReceiveShock(extraDamage);

                if (previousHealth > 0 && m.MobileMetadata.CurrentHealth <= 0)
                {
                    m.RequestDeath(CausaMortis.NoHealth);
                    wasTargetKilled = true;
                }

                Mobile.ComputeTurnDamage(this, m, extraDamage, wasTargetKilled);

                OnDealDamageAction?.Invoke(extraDamage);
            }
        }
    }

    public class LightningBaseProjectile : BeamDummyProjectile
    {
        public LightningBaseProjectile(Mobile mobile, Vector2 parentPosition, float beamAngle, int explosionRadius, int extraExplosionRadius, int baseDamage, int extraDamage)
            : base(mobile, parentPosition, beamAngle, explosionRadius, extraExplosionRadius, baseDamage, extraDamage, BeamEmitterType.Lightning) { }

        public override void Explode()
        {
            base.Explode();

            LevelScene.WeatherHandler.Add(new LightningElectricity(Position, MathHelper.PiOver2 * 3 - beamAngle));
        }
    }
}
