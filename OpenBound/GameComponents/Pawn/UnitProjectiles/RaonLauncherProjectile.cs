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
using OpenBound.GameComponents.Level;
using OpenBound.GameComponents.Level.Scene;
using OpenBound.GameComponents.Pawn.Unit;
using OpenBound.GameComponents.MobileAction;
using OpenBound.GameComponents.WeatherEffect;
using OpenBound_Network_Object_Library.Entity;
using OpenBound_Network_Object_Library.Entity.Sync;
using OpenBound.Extension;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenBound.GameComponents.Pawn.UnitProjectiles
{
    public class RaonLauncherProjectileEmitter
    {
        public static void Shot1(RaonLauncher mobile)
        {
            mobile.UninitializedProjectileList.Add(new RaonBaseProjectile1(mobile, new Vector2(12, 0), 1 * MathHelper.PiOver2));
            mobile.UninitializedProjectileList.Add(new RaonBaseProjectile1(mobile, new Vector2(12, 0), 2 * MathHelper.PiOver2));
            mobile.UninitializedProjectileList.Add(new RaonBaseProjectile1(mobile, new Vector2(12, 0), 3 * MathHelper.PiOver2));
            mobile.UninitializedProjectileList.Add(new RaonBaseProjectile1(mobile, new Vector2(12, 0), 4 * MathHelper.PiOver2));
        }

        public static void Shot2(RaonLauncher mobile)
        {
            mobile.UninitializedProjectileList.Add(new RaonProjectile2(mobile,  3));
            mobile.UninitializedProjectileList.Add(new RaonProjectile2(mobile, -3));
        }
    }

    public class RaonProjectile1 : Projectile
    {
        public RaonProjectile1(RaonLauncher mobile)
            : base(mobile, ShotType.S1, Parameter.ProjectileRaonLauncherS1ExplosionRadius, Parameter.ProjectileRaonLauncherS1BaseDamage)
        {
            //Initializing Flipbook
            FlipbookList.Add(new Flipbook(
                mobile.Crosshair.CannonPosition, new Vector2(19, 21),
                38, 42, "Graphics/Tank/RaonLauncher/Bullet1",
                new List<AnimationInstance>() { new AnimationInstance() { StartingFrame = 0, EndingFrame = 39, TimePerFrame = 1 / 15f } }, DepthParameter.Projectile, angle));

            FlipbookList[0].SetTransparency(0);
        }

        public override void Explode()
        {
            base.Explode();
            SpecialEffectBuilder.RaonLauncherProjectile1Explosion(FlipbookList[0].Position);
        }
    }

    public class RaonBaseProjectile1 : DummyProjectile
    {
        //Special Effects
        List<SpecialEffect> specialEffectList, toBeRemovedSpecialEffectList;
        Vector2 lastSESpawnPosition;

        RaonProjectile1 projectile;
        float rotationAngle, offsetFactor;
        Vector2 offset;

        public RaonBaseProjectile1(RaonLauncher mobile, Vector2 positionOffset, float rotationAngle)
            : base(mobile, ShotType.S1, 0, 0, canCollide: false)
        {
            projectile = new RaonProjectile1(mobile);

            specialEffectList = new List<SpecialEffect>();
            toBeRemovedSpecialEffectList = new List<SpecialEffect>();

            lastSESpawnPosition = mobile.Crosshair.CannonPosition;

            this.rotationAngle = rotationAngle;
            offset = positionOffset;

            SpawnTime = 0.7f;

            projectile.OnFinalizeExecutionAction = OnFinalizeExecutionAction;

            //Physics/Trajectory setups
            mass = Parameter.ProjectileRaonLauncherS1Mass;
            windInfluence = Parameter.ProjectileRaonLauncherS1WindInfluence;
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

                rotationAngle -= 2 * Parameter.ProjectileMovementTimeElapsedPerInteraction * MathHelper.Pi;

                projectile.FlipbookList[0].Position = Position + offsetFactor * Vector2.Transform(offset * (float)Math.Sin(rotationAngle), Matrix.CreateRotationZ(FlipbookList[0].Rotation + MathHelper.PiOver2));

                if (projectile.CheckOutOfBounds(projectile.FlipbookList[0].Position) || projectile.UpdateCollider(projectile.FlipbookList[0].Position))
                {
                    Destroy();
                    break;
                }

                if (Helper.SquaredEuclideanDistance(lastSESpawnPosition, projectile.Position) > 500)
                {
                    specialEffectList.Add(SpecialEffectBuilder.RaonLauncherProjectile1(projectile.Position, projectile.CurrentFlipbookRotation));
                    lastSESpawnPosition = projectile.Position;
                }
            }

            //Animation
            float transparency = 1;
            for (int i = specialEffectList.Count - 1; i >= 0; i--)
            {
                specialEffectList[i].Flipbook.SetCurrentFrame(projectile.FlipbookList[0].GetCurrentFrame());
                //I am using color mult because of weakness
                specialEffectList[i].Flipbook.Color = projectile.FlipbookList[0].Color * (transparency -= 0.05f);

                if (specialEffectList[i].Flipbook.Color == Color.Transparent)
                    toBeRemovedSpecialEffectList.Add(specialEffectList[i]);
            }

            toBeRemovedSpecialEffectList.ForEach((x) => specialEffectList.Remove(x));
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);
            projectile.Draw(gameTime, spriteBatch);
            specialEffectList.ForEach((x) => x.Flipbook.Draw(gameTime, spriteBatch));
        }
    }

    public class RaonProjectile2 : Projectile
    {
        public RaonProjectile2(RaonLauncher mobile, float forceModifier)
            : base(mobile, ShotType.S2, 0, 0, forceModifier: forceModifier)
        {
            //Initializing Flipbook
            FlipbookList.Add(new Flipbook(
                mobile.Crosshair.CannonPosition, new Vector2(15f, 15f),
                33, 31, "Graphics/Tank/RaonLauncher/Bullet2",
                new List<AnimationInstance>() {
                    new AnimationInstance()
                    { StartingFrame = 0, EndingFrame = 9, TimePerFrame = 1 / 20f }
                }, DepthParameter.Projectile, angle));

            //Physics/Trajectory setups
            mass = Parameter.ProjectileRaonLauncherS2Mass;
            windInfluence = Parameter.ProjectileRaonLauncherS2WindInfluence;

            SpawnTime = 0.7;
        }

        public override void Explode()
        {
            base.Explode();
            LevelScene.MineList.Add(new RaonLauncherMineS2(Mobile, previousPosition.ToIntegerDomain()));
        }

        protected override void Destroy()
        {
            base.Destroy();

            List<Projectile> pjList = Mobile.ProjectileList.Except(Mobile.UnusedProjectile).ToList();

            if (pjList.Count() == 0)
                OnFinalizeExecutionAction?.Invoke();
        }

        public override bool UpdateCollider(Vector2 position)
        {
            bool hasExploded = false;

            //Check collision with ground
            if (CanCollide && Topography.CheckCollision(position))
            {
                hasExploded = true;
                Explode();
#if Debug
                debugCrosshair.Update(FlipbookList[0].Position);
#endif
            }

            return hasExploded;
        }
    }

    //Projectile Spawned/Exploded when a mine touches a mobile
    public class RaonProjectile2Explosion : DummyProjectile
    {
        public RaonProjectile2Explosion(RaonLauncher mobile)
         : base(mobile, ShotType.S2, Parameter.ProjectileRaonLauncherS2ExplosionRadius, Parameter.ProjectileRaonLauncherS2BaseDamage)
        {
            //Physics/Trajectory setups
            mass = 1; windInfluence = 1;
        }

        public override void Explode()
        {
            CheckCollisionWithWeather();
            base.Explode();
            SpecialEffectBuilder.RaonLauncherProjectile2Explosion(Position);
        }
    }

    public class RaonProjectile3 : Projectile
    {
        public RaonProjectile3(RaonLauncher mobile) : base(mobile, ShotType.SS, 0, 0)
        {
            Mobile = mobile;

            //Initializing Flipbook
            FlipbookList.Add(new Flipbook(
                mobile.Crosshair.CannonPosition, new Vector2(21f, 16f),
                44, 34, "Graphics/Tank/RaonLauncher/Bullet3",
                new List<AnimationInstance>() {
                    new AnimationInstance(){ StartingFrame = 0, EndingFrame = 9, TimePerFrame = 1 / 20f }
                }, DepthParameter.Projectile, angle));

            //Physics/Trajectory setups
            mass = Parameter.ProjectileIceSSMass;
            windInfluence = Parameter.ProjectileIceSSWindInfluence;

            SpawnTime = 0.7;
        }

        public override void Explode()
        {
            //Wipe the "pass turn" action from the projectile and pass the responsability to the mine.
            Action explodeAction = OnFinalizeExecutionAction;
            OnFinalizeExecutionAction = default;

            base.Explode();

            // The mine must be added on the scene AFTER the explosion
            // to preventing it from being exploded by the weather
            RaonLauncherMineSS rlmss = new RaonLauncherMineSS(Mobile, previousPosition.ToIntegerDomain(), explodeAction);
            LevelScene.MineList.Add(rlmss);
        }

        protected override void Destroy()
        {
            base.Destroy();

            List<Projectile> pjList = Mobile.ProjectileList.Except(Mobile.UnusedProjectile).ToList();

            if (pjList.Count() == 0)
                OnFinalizeExecutionAction?.Invoke();
        }

        public override bool UpdateCollider(Vector2 position)
        {
            bool hasExploded = false;

            //Check collision with ground
            if (CanCollide && Topography.CheckCollision(position))
            {
                hasExploded = true;
                Explode();
#if Debug
                debugCrosshair.Update(FlipbookList[0].Position);
#endif
            }

            return hasExploded;
        }
    }

    public class RaonProjectile3Explosion : DummyProjectile
    {
        public RaonProjectile3Explosion(RaonLauncher mobile)
         : base(mobile, ShotType.SS, Parameter.ProjectileRaonLauncherSSExplosionRadius, Parameter.ProjectileRaonLauncherSSBaseDamage)
        {
            //Physics/Trajectory setups
            mass = 1; windInfluence = 1;
        }

        public override void Explode()
        {
            CheckCollisionWithWeather();
            base.Explode();
            SpecialEffectBuilder.RaonLauncherProjectile3Explosion(Position);
        }
    }
}