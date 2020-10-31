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
using OpenBound.Common;
using OpenBound.GameComponents.Animation;
using OpenBound.GameComponents.Pawn.Unit;
using OpenBound.GameComponents.MobileAction;
using OpenBound_Network_Object_Library.Entity;
using System.Collections.Generic;
using System.Linq;
using OpenBound.GameComponents.Level.Scene;
using System;

namespace OpenBound.GameComponents.Pawn.UnitProjectiles
{

    public class IceProjectile1 : Projectile
    {
        public IceProjectile1(Ice mobile) : base(mobile, ShotType.S1, Parameter.ProjectileIceS1ExplosionRadius, Parameter.ProjectileIceS1BaseDamage)
        {
            //Initializing Flipbook
            FlipbookList.Add(new Flipbook(
                mobile.Crosshair.CannonPosition, new Vector2(17.5f, 12),
                35, 24, "Graphics/Tank/Ice/Bullet1",
                new List<AnimationInstance>() {
                    new AnimationInstance()
                    { StartingFrame = 0, EndingFrame = 19, TimePerFrame = 1 / 20f }
                }, DepthParameter.Projectile, angle));

            //Physics/Trajectory setups
            mass = Parameter.ProjectileIceS1Mass;
            windInfluence = Parameter.ProjectileIceS1WindInfluence;

            SpawnTime = 0.7f;
        }

        public override void Explode()
        {
            base.Explode();
            SpecialEffectBuilder.IceProjectile1Explosion(FlipbookList[0].Position, (float)Parameter.Random.NextDouble() * MathHelper.TwoPi);
        }

        protected override void Destroy()
        {
            base.Destroy();

            List<Projectile> pjList = Mobile.ProjectileList.Except(Mobile.UnusedProjectile).ToList();

            if (pjList.Count() == 0)
                OnFinalizeExecutionAction?.Invoke();
        }
    }

    public class IceProjectile2 : Projectile
    {
        public IceProjectile2(Ice mobile) : base(mobile, ShotType.S2, Parameter.ProjectileIceS2ExplosionRadius, Parameter.ProjectileIceS2BaseDamage)
        {
            //Initializing Flipbook
            FlipbookList.Add(new Flipbook(
                mobile.Crosshair.CannonPosition, new Vector2(16.5f, 17f),
                37, 34, "Graphics/Tank/Ice/Bullet2",
                new List<AnimationInstance>() {
                    new AnimationInstance()
                    { StartingFrame = 0, EndingFrame = 11, TimePerFrame = 1 / 20f }
                }, DepthParameter.Projectile, angle));

            //Physics/Trajectory setups
            mass = Parameter.ProjectileIceS2Mass;
            windInfluence = Parameter.ProjectileIceS2WindInfluence;

            SpawnTime = 0.2;
        }

        public override void Explode()
        {
            SpecialEffectBuilder.IceProjectile2Explosion(FlipbookList[0].Position, FlipbookList[0].Rotation);
            base.Explode();
        }

        protected override void OnDealDamage(Mobile mobile, int damage)
        {
            base.OnDealDamage(mobile, damage);

            //Add into ice's armor reduction dictionary
            Dictionary<Mobile, int> ard = ((Ice)Mobile).ArmorReductionDictionary;

            if (!ard.ContainsKey(mobile))
                ard.Add(mobile, 0);

            //If it was exceeded the maximum possible reduction, return
            if (ard[mobile] <= Parameter.ProjectileIceS2MaximumDefenseReduction)
                return;

            ard[mobile] += Parameter.ProjectileIceS2DefenseReduction;
            mobile.Owner.Defense += Parameter.ProjectileIceS2DefenseReduction;

            LevelScene.HUD.StatusBarDictionary[mobile].UpdateAttributeList();
        }

        protected override void Destroy()
        {
            base.Destroy();

            List<Projectile> pjList = Mobile.ProjectileList.Except(Mobile.UnusedProjectile).ToList();

            if (pjList.Count() == 0)
                OnFinalizeExecutionAction?.Invoke();
        }
    }

    public class IceProjectile3 : Projectile
    {
        public IceProjectile3(Ice mobile) : base(mobile, ShotType.SS, Parameter.ProjectileIceSSExplosionRadius, Parameter.ProjectileIceSSBaseDamage)
        {
            //Initializing Flipbook
            FlipbookList.Add(new Flipbook(
                mobile.Crosshair.CannonPosition, new Vector2(38.5f, 35f),
                77, 70, "Graphics/Tank/Ice/Bullet3",
                new List<AnimationInstance>() {
                    new AnimationInstance(){ StartingFrame = 0, EndingFrame = 14, TimePerFrame = 1 / 20f }
                }, DepthParameter.Projectile, angle));

            //Physics/Trajectory setups
            mass = Parameter.ProjectileIceSSMass;
            windInfluence = Parameter.ProjectileIceSSWindInfluence;

            SpawnTime = 0.7;
        }

        public override void Explode()
        {
            SpecialEffectBuilder.IceProjectile3Explosion(FlipbookList[0].Position);
            base.Explode();
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