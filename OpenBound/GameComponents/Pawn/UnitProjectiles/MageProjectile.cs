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
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace OpenBound.GameComponents.Pawn.UnitProjectiles
{
    public class MageProjectileEmitter
    {
        //This method was created to prevent hurricane positions
        public static void Shot2(Mage mobile)
        {
            mobile.UninitializedProjectileList.Add(new MageProjectile2(mobile, 0, new Color(200, 60, 100, 60)));
            mobile.UninitializedProjectileList.Add(new MageProjectile2(mobile, MathHelper.Pi, new Color(150, 150, 200, 100)));
        }
    }

    public class MageProjectile1 : DummyProjectile
    {
        HelicoidalTrace trace;

        public MageProjectile1(Mage mobile)
            : base(mobile, ShotType.S1, Parameter.ProjectileMageS1ExplosionRadius, Parameter.ProjectileMageS1BaseDamage)
        {
            //Physics/Trajectory setups
            mass = Parameter.ProjectileMageS1Mass;
            windInfluence = Parameter.ProjectileMageS1WindInfluence;

            trace = new HelicoidalTrace(MobileType.Mage, ShotType.S1, Color.White, this);
        }

        public override void Update()
        {
            base.Update();
            if (InteractionTimeCounter < InteractionTime) return;
            trace.Update(FlipbookList[0].Position, Vector2.Zero, FlipbookList[0].Rotation, 0, 0);
        }

        public override void Explode()
        {
            base.Explode();
            SpecialEffectBuilder.MageProjectile1Explosion(FlipbookList[0].Position, 0);
        }

        #region Weather
        //Weakness
        public override void OnBeginWeaknessInteraction(Weakness weakness)
        {
            trace.Color = Parameter.WeatherEffectWeaknessColorModifier;
        }
        #endregion

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (InteractionTimeCounter < InteractionTime || SpawnTimeCounter < SpawnTime) return;
            base.Draw(gameTime, spriteBatch);
            trace.Draw(gameTime, spriteBatch);
        }
    }

    public class MageProjectile2 : DummyProjectile
    {
        HelicoidalTrace trace;
        float angleOffset;

        DummyProjectile dProj;

        Vector2 traceOffset;

        public MageProjectile2(Mage mobile, float angleOffset, Color color)
            : base(mobile, ShotType.S2, 0, 0, canCollide: false)
        {
            trace = new HelicoidalTrace(MobileType.Mage, ShotType.S2, color, this);
            this.angleOffset = angleOffset;

            mass = Parameter.ProjectileMageS2Mass;
            windInfluence = Parameter.ProjectileMageS2WindInfluence;

            dProj = new DummyProjectile(mobile, ShotType.S2, Parameter.ProjectileMageS2ExplosionRadius, Parameter.ProjectileMageS2BaseDamage);
            //dProj.IsAbleToRefreshPosition = false;
            //dProj.IsExternallyRefreshingPosition = false;

            traceOffset = new Vector2(0, 15);
        }

        public override void Explode()
        {
            base.Explode();
            SpecialEffectBuilder.MageProjectile2Explosion(trace.Position, 0);
        }

        #region Weather
        protected override void CheckCollisionWithWeather()
        {
            foreach (Weather w in LevelScene.WeatherHandler.WeatherList)
            {
                if (w.Intersects(dProj) && !w.IsInteracting(this))
                {
                    Position = dProj.Position;
                    w.ModifiedProjectileList.Add(this);
                    w.OnInteract(this);
                }
            }
        }

        //Force
        public override void OnBeginForceInteraction(Force force)
        {
            force.OnInteract(dProj);
        }

        //Weakness
        public override void OnBeginWeaknessInteraction(Weakness weakness)
        {
            weakness.OnInteract(dProj);
            trace.Color = Parameter.WeatherEffectWeaknessColorModifier;
        }

        //Electricity
        public override void OnBeginElectricityInteraction(Electricity electricity)
        {
            //electricity.OnInteract(this);
            electricity.OnInteract(dProj);
            OnAfterUpdateAction = dProj.OnAfterUpdateAction;
        }

        //Random
        public override void OnBeginRandomInteraction(WeatherEffect.Random random)
        {
            random.OnInteract(dProj);
        }

        //Thor
        public override void OnBeginThorInteraction(Thor thor)
        {
            thor.OnInteract(dProj);
        }
        #endregion

        public override void Update()
        {
            base.Update();
            if (InteractionTimeCounter < InteractionTime) return;

            for (float i = 0; i < Parameter.ProjectileMovementTotalTimeElapsed; i += Parameter.ProjectileMovementTimeElapsedPerInteraction)
            {
                trace.Update(FlipbookList[0].Position, traceOffset, FlipbookList[0].Rotation, Parameter.ProjectileMovementTimeElapsedPerInteraction, angleOffset);

                dProj.FlipbookList[0].Position = trace.Position;

                if (dProj.CheckOutOfBounds(trace.Position))
                    return;

                if (dProj.UpdateCollider(trace.Position))
                {
                    Explode();
                    return;
                }
            }
        }

        public override bool CheckOutOfBounds(Vector2 position)
        {
            return base.CheckOutOfBounds(trace.Position);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (InteractionTimeCounter < InteractionTime || SpawnTimeCounter < SpawnTime) return;
            base.Draw(gameTime, spriteBatch);
            trace.Draw(gameTime, spriteBatch);
        }
    }

    public class MageProjectile3 : Projectile
    {
        public MageProjectile3(Mage mobile)
            : base(mobile, ShotType.S1, Parameter.ProjectileMageS2ExplosionRadius, Parameter.ProjectileMageS2BaseDamage)
        {
            //Initializing Flipbook
            FlipbookList.Add(new Flipbook(
                mobile.Crosshair.CannonPosition, new Vector2(27, 27),
                54, 54, "Graphics/Tank/Mage/Bullet3",
                new AnimationInstance() { StartingFrame = 0, EndingFrame = 14, TimePerFrame = 1 / 20f },
                DepthParameter.Projectile, angle));

            //Physics/Trajectory setups
            mass = Parameter.ProjectileMageS2Mass;
            windInfluence = Parameter.ProjectileMageS2WindInfluence;
        }

        protected override int CalculateDamage(Mobile mobile)
        {
            double distance = mobile.CollisionBox.GetDistance(FlipbookList[0].Position, ExplosionRadius);

            if (distance < Parameter.ProjectileMageSSEExplosionRadius)
            {
                return base.CalculateDamage(mobile) + (int)mobile.SyncMobile.MobileMetadata.CurrentShield;
            }

            return 0;
        }

        public override void Explode()
        {
            base.Explode();

            SpecialEffectBuilder.MageProjectile3Explosion(FlipbookList[0].Position, FlipbookList[0].Rotation);

            foreach (Mobile m in LevelScene.MobileList)
            {
                double distance = m.CollisionBox.GetDistance(FlipbookList[0].Position, ExplosionRadius);

                if (distance < Parameter.ProjectileMageSSEExplosionRadius)
                {
                    m.DepleteShield();
                }
            }
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
