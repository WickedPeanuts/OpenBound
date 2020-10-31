/* 
 * Copyright (C) 2020, Carlos H.M.S. <carlos_judo@hotmail.com>
 * This file is part of OpenBound.
 * OpenBound is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation, either version 3 of the License, or(at your option) any later version.
 * 
 * OpenBound is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty
 * of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License along with OpenBound. If not, see http://www.gnu.org/licenses/.
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpenBound.Common;
using OpenBound.GameComponents.Animation;
using OpenBound.GameComponents.Level.Scene;
using OpenBound.GameComponents.Pawn.Unit;
using OpenBound.GameComponents.MobileAction;
using OpenBound.GameComponents.WeatherEffect;
using OpenBound_Network_Object_Library.Entity;
using OpenBound_Network_Object_Library.Entity.Sync;
using System.Collections.Generic;
using System.Linq;

namespace OpenBound.GameComponents.Pawn.UnitProjectiles
{
    public class TricoProjectileEmitter
    {
        public static void Shot2(Trico mobile)
        {
            float angleModifier = mobile.Facing == Facing.Left ? -1 : 1;
            mobile.UninitializedProjectileList.Add(new TricoBaseProjectile2(mobile, new Vector2( 35, 0), angleModifier));
            mobile.UninitializedProjectileList.Add(new TricoBaseProjectile2(mobile, Vector2.Zero,        angleModifier));
            mobile.UninitializedProjectileList.Add(new TricoBaseProjectile2(mobile, new Vector2(-35, 0), angleModifier));
        }
    }

    public class TricoProjectile1 : Projectile
    {
        public TricoProjectile1(Trico mobile)
            : base(mobile, ShotType.S1, Parameter.ProjectileTricoS1ExplosionRadius, Parameter.ProjectileTricoS1BaseDamage)
        {
            //Initializing Flipbook
            FlipbookList.Add(new Flipbook(
                mobile.Crosshair.CannonPosition, new Vector2(16, 16),
                32, 32, "Graphics/Tank/Trico/Bullet1",
                new List<AnimationInstance>() {
                    new AnimationInstance()
                    { StartingFrame = 0, EndingFrame = 7, TimePerFrame = 1 / 20f }
                }, DepthParameter.Projectile, angle));

            //Physics/Trajectory setups
            mass = Parameter.ProjectileTricoS1Mass;
            windInfluence = Parameter.ProjectileTricoS1WindInfluence;

            SpawnTime = 0.7;
        }

        public override void Explode()
        {
            base.Explode();
            SpecialEffectBuilder.TricoProjectile1Explosion(FlipbookList[0].Position, FlipbookList[0].Rotation);
        }

        protected override void Destroy()
        {
            base.Destroy();

            List<Projectile> pjList = Mobile.ProjectileList.Except(Mobile.UnusedProjectile).ToList();

            if (pjList.Count() == 0)
                OnFinalizeExecutionAction?.Invoke();
        }
    }

    public class TricoProjectile2 : Projectile
    {
        public TricoProjectile2(Trico mobile)
            : base(mobile, ShotType.S2, Parameter.ProjectileTricoS2ExplosionRadius, Parameter.ProjectileTricoS2BaseDamage)
        {
            //Initializing Flipbook
            FlipbookList.Add(new Flipbook(
                mobile.Crosshair.CannonPosition, new Vector2(16, 16),
                32, 32, "Graphics/Tank/Trico/Bullet2",
                new List<AnimationInstance>() { new AnimationInstance() { StartingFrame = 0, EndingFrame = 7, TimePerFrame = 1 / 20f } }, 
                DepthParameter.Projectile, angle));

            FlipbookList[0].SetTransparency(0);
        }

        public override void Explode()
        {
            base.Explode();
            SpecialEffectBuilder.TricoProjectile2Explosion(FlipbookList[0].Position, FlipbookList[0].Rotation);
        }
    }

    public class TricoBaseProjectile2 : DummyProjectile
    {
        TricoProjectile2 projectile;

        float angleModifier;
        float rotationAngle;
        float offsetFactor;
        Vector2 offset;

        public TricoBaseProjectile2(Trico mobile, Vector2 positionOffset, float angleModifier)
            : base(mobile, ShotType.S2, 0, 0, canCollide: false)
        {
            projectile = new TricoProjectile2(mobile);

            rotationAngle = 0;
            offset = positionOffset;
            this.angleModifier = angleModifier;

            SpawnTime = 0.7f;
            offsetFactor = 0;

            projectile.OnFinalizeExecutionAction = OnFinalizeExecutionAction;

            //Physics/Trajectory setups
            mass = Parameter.ProjectileTricoS2Mass;
            windInfluence = Parameter.ProjectileTricoS2WindInfluence;
        }

        #region Weather
        protected override void CheckCollisionWithWeather()
        {
            foreach (Weather w in LevelScene.WeatherHandler.WeatherList)
            {
                if (w.Intersects(projectile) && !w.IsInteracting(this))
                {
                    Position = projectile.Position;
                    w.ModifiedProjectileList.Add(this);
                    w.OnInteract(this);
                }
            }
        }

        //Force
        public override void OnBeginForceInteraction(Force force)
        {
            force.OnInteract(projectile);
        }

        //Weakness
        public override void OnBeginWeaknessInteraction(Weakness weakness)
        {
            weakness.OnInteract(projectile);
        }

        //Electricity
        public override void OnBeginElectricityInteraction(Electricity electricity)
        {
            //electricity.OnInteract(this);
            electricity.OnInteract(projectile);
            OnAfterUpdateAction = projectile.OnAfterUpdateAction;
        }

        //Random
        public override void OnBeginRandomInteraction(WeatherEffect.Random random)
        {
            random.OnInteract(projectile);
        }

        //Thor
        public override void OnBeginThorInteraction(Thor thor)
        {
            thor.OnInteract(projectile);
        }
        #endregion

        public override void OnSpawn()
        {
            base.OnSpawn();
            projectile.FlipbookList[0].SetTransparency(1);
        }

        protected override void UpdateRotation()
        {
            base.UpdateRotation();
            projectile.FlipbookList[0].Rotation = FlipbookList[0].Rotation;
        }

        protected override void UpdatePosition()
        {
            for (float i = 0; i < Parameter.ProjectileMovementTotalTimeElapsed; i += Parameter.ProjectileMovementTimeElapsedPerInteraction)
            {
                if (IsExternallyRefreshingPosition)
                {
                    offsetFactor = MathHelper.Max(offsetFactor - 2 * Parameter.ProjectileMovementTimeElapsedPerInteraction, 0);
                }
                else
                {
                    offsetFactor = MathHelper.Min(offsetFactor + 2 * Parameter.ProjectileMovementTimeElapsedPerInteraction, 1);
                }

                base.UpdateMovementIteraction(Parameter.ProjectileMovementTimeElapsedPerInteraction);

                rotationAngle += Parameter.ProjectileMovementTimeElapsedPerInteraction * MathHelper.Pi;

                projectile.FlipbookList[0].Position = Position + Vector2.Transform(offset * offsetFactor, Matrix.CreateRotationZ(rotationAngle * angleModifier));

                if (projectile.CheckOutOfBounds(projectile.FlipbookList[0].Position) || projectile.UpdateCollider(projectile.FlipbookList[0].Position))
                {
                    Destroy();
                    break;
                }
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);
            projectile.Draw(gameTime, spriteBatch);
        }
    }

    public class TricoProjectile3 : Projectile
    {
        Vector2 baseCoordinate;
        bool hasExploded;
        float explosionTimer;
        float rotationExplosionOffset;
        float baseRotation;
        int explosions;

        public TricoProjectile3(Trico mobile)
            : base(mobile, ShotType.SS, Parameter.ProjectileTricoSSExplosionRadius, Parameter.ProjectileTricoSSBaseDamage)
        {
            Mobile = mobile;

            rotationExplosionOffset = mobile.Facing == Facing.Left ? -1 : 1;

            //Initializing Flipbook
            FlipbookList.Add(new Flipbook(
                mobile.Crosshair.CannonPosition, new Vector2(40, 17),
                66, 36, "Graphics/Tank/Trico/Bullet3",
                new AnimationInstance() { StartingFrame = 0, EndingFrame = 7, TimePerFrame = 1/19f },
                DepthParameter.Projectile, angle));

            hasExploded = false;
            explosionTimer = 0;

            //Physics/Trajectory setups
            mass = Parameter.ProjectileTricoSSMass;
            windInfluence = Parameter.ProjectileTricoSSWindInfluence;

            SpawnTime = 0.5f;
        }

        #region Weather
        //Electricity
        public override void OnBeginElectricityInteraction(Electricity electricity)
        {
            //Clear the actions to prevent spawning multiple bolts
            OnExplodeAction += () => { OnExplodeAction = default; };
        }

        //Electricity
        public override void OnBeginThorInteraction(Thor thor)
        {
            base.OnBeginThorInteraction(thor);

            //Clear the actions to prevent spawning multiple thor shots
            OnExplodeAction += () => { OnExplodeAction = default; };
        }
        #endregion

        protected override void UpdatePosition()
        {       
            //Interpolate Movement for collision
            for (float elapsedTime = 0; elapsedTime < Parameter.ProjectileMovementTotalTimeElapsed; elapsedTime += Parameter.ProjectileMovementTimeElapsedPerInteraction)
            {
                if (!hasExploded)
                    UpdateMovementIteraction(Parameter.ProjectileMovementTimeElapsedPerInteraction);

                if (CheckOutOfBounds(Position))
                    break;
                
                if (hasExploded || UpdateCollider(Position))
                {
                    if (!hasExploded)
                    {
                        hasExploded = true;
                        baseCoordinate = Position;
                        baseRotation = FlipbookList[0].Rotation;
                        SpecialEffectBuilder.TricoProjectile3Explosion(FlipbookList[0].Position);
                        FlipbookList[0].SetTransparency(0);
                    }

                    if ((explosionTimer += Parameter.ProjectileMovementTimeElapsedPerInteraction) > Parameter.ProjectileTricoSSExtraBlastTime)
                    {
                        explosionTimer = 0;

                        FlipbookList[0].Position = baseCoordinate +
                            Vector2.Transform(
                                Parameter.ProjectileTricoSSExplosionOffset,
                                Matrix.CreateRotationZ(
                                    baseRotation + explosions * (explosions % 2 == 0 ? 1 : -1) * Parameter.ProjectileTricoSSRotationOffset * rotationExplosionOffset));
                        
                        explosions++;
                        Explode();
                    }
                }
            }

#if Debug
            debugCrosshair.Update(FlipbookList[0].Position);
#endif
        }

        protected override void OutofboundsDestroy()
        {
            explosions = Parameter.ProjectileTricoSSNumberOfExplosions;
            base.OutofboundsDestroy();
        }

        protected override void Destroy()
        {
            if (explosions >= Parameter.ProjectileTricoSSNumberOfExplosions)
            {
                base.Destroy();

                List<Projectile> pjList = Mobile.ProjectileList.Except(Mobile.UnusedProjectile).ToList();

                if (pjList.Count() == 0)
                    OnFinalizeExecutionAction?.Invoke();
            }
        }
    }
}
