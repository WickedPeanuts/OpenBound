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

namespace OpenBound.GameComponents.Pawn.UnitProjectiles
{
    public class LightningProjectile1 : Projectile
    {
        public LightningProjectile1(Lightning mobile) : base(mobile, ShotType.S1, Parameter.ProjectileLightningS1ExplosionRadius, Parameter.ProjectileLightningS1BaseDamage)
        {
            //Initializing Flipbook
            FlipbookList.Add(new Flipbook(
                mobile.Crosshair.CannonPosition, new Vector2(30f, 31),
                53, 60, "Graphics/Tank/Lightning/Bullet1",
                new List<AnimationInstance>() {
                    new AnimationInstance()
                    { StartingFrame = 0, EndingFrame = 9, TimePerFrame = 1 / 20f }
                }, DepthParameter.Projectile, angle));

            //Physics/Trajectory setups
            mass = Parameter.ProjectileLightningS1Mass;
            windInfluence = Parameter.ProjectileLightningS1WindInfluence;
        }

        public override void Explode()
        {
            base.Explode();

            LightningBaseProjectile electricityProjectile =
                new LightningBaseProjectile(Mobile, Position,
                    Parameter.ProjectileLightningS1ElectricityAngle,
                    Parameter.ProjectileLightningS1ElectricityExplosionRadius,
                    Parameter.ProjectileLightningS1ElectricityEExplosionRadius,
                    Parameter.ProjectileLightningS1ElectricityBaseDamage,
                    Parameter.ProjectileLightningS1ElectricityEBaseDamage);

            electricityProjectile.Update();
        }

        protected override void Destroy()
        {
            base.Destroy();

            List<Projectile> pjList = Mobile.ProjectileList.Except(Mobile.UnusedProjectile).ToList();

            if (pjList.Count() == 0)
                OnFinalizeExecutionAction?.Invoke();
        }

        public override void Draw(GameTime GameTime, SpriteBatch SpriteBatch)
        {
            base.Draw(GameTime, SpriteBatch);
        }
    }

    public class LightningProjectile2 : Projectile
    {
        public LightningProjectile2(Lightning mobile) : base(mobile, ShotType.S2, Parameter.ProjectileLightningS2ExplosionRadius, Parameter.ProjectileLightningS2BaseDamage)
        {
            Mobile = mobile;

            //Initializing Flipbook
            FlipbookList.Add(new Flipbook(
                mobile.Crosshair.CannonPosition, new Vector2(30f, 31),
                53, 60, "Graphics/Tank/Lightning/Bullet1",
                new List<AnimationInstance>() {
                    new AnimationInstance()
                    { StartingFrame = 0, EndingFrame = 9, TimePerFrame = 1 / 20f }
                }, DepthParameter.Projectile));

            //Physics/Trajectory setups
            mass = Parameter.ProjectileLightningS2Mass;
            windInfluence = Parameter.ProjectileLightningS2WindInfluence;

            SpawnTime = 0.2;
        }

        public override void Explode()
        {
            base.Explode();

            for (int i = 0; i <= 3; i++)
            {
                LightningBaseProjectile electricityProjectile = new LightningBaseProjectile(Mobile, Position,
                  Parameter.ProjectileLightningS2AnglesOffset[i],
                  Parameter.ProjectileLightningS2ElectricityExplosionRadius,
                  Parameter.ProjectileLightningS2ElectricityEExplosionRadius,
                  Parameter.ProjectileLightningS2ElectricityBaseDamage,
                  Parameter.ProjectileLightningS2ElectricityEBaseDamage);
                electricityProjectile.Update();
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

    public class LightningProjectile3 : Projectile
    {
        public LightningProjectile3(Lightning mobile) : base(mobile, ShotType.SS, Parameter.ProjectileLightningSSExplosionRadius, Parameter.ProjectileLightningSSBaseDamage)
        {
            Mobile = mobile;

            //Initializing Flipbook
            FlipbookList.Add(new Flipbook(
                mobile.Crosshair.CannonPosition, new Vector2(44f, 27f),
                99, 50, "Graphics/Tank/Lightning/Bullet3",
                new List<AnimationInstance>() {
                    new AnimationInstance(){ StartingFrame = 0, EndingFrame = 11, TimePerFrame = 1 / 20f }
                }, DepthParameter.Projectile));

            //Physics/Trajectory setups
            mass = Parameter.ProjectileLightningSSMass;
            windInfluence = Parameter.ProjectileLightningSSWindInfluence;

            SpawnTime = 0;
        }

        public override void Explode()
        {
            SpecialEffectBuilder.LightningProjectile3Explosion(FlipbookList[0].Position, FlipbookList[0].Rotation);
            base.Explode();

            foreach (Mobile m in LevelScene.MobileList)
            {
                double distance = m.CollisionBox.GetDistance(FlipbookList[0].Position, ExplosionRadius);

                if (distance < Parameter.ProjectileLightningSSEExplosionRadius)
                {
                    LightningBaseProjectile electricityProjectile =
                      new LightningBaseProjectile(Mobile, m.Position,
                          Parameter.ProjectileLightningSSElectricityAngle,
                          Parameter.ProjectileLightningSSElectricityExplosionRadius,
                          Parameter.ProjectileLightningSSElectricityEExplosionRadius,
                          Parameter.ProjectileLightningSSElectricityBaseDamage,
                          Parameter.ProjectileLightningSSElectricityEBaseDamage);
                    electricityProjectile.Update();
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



