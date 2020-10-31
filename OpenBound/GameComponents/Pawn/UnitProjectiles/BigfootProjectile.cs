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
using OpenBound.GameComponents.Level.Scene;
using OpenBound.GameComponents.Pawn.Unit;
using OpenBound.GameComponents.MobileAction;
using OpenBound_Network_Object_Library.Entity;
using OpenBound_Network_Object_Library.Entity.Sync;
using System.Collections.Generic;
using System.Linq;

namespace OpenBound.GameComponents.Pawn.UnitProjectiles
{
    public class BigfootProjectileEmitter
    {
        public static void Shot1(Bigfoot mobile)
        {
            int factor = mobile.Facing == Facing.Left ? 1 : -1;
            int i = 0;
            mobile.UninitializedProjectileList.Add(new BigfootProjectile1(mobile, MathHelper.ToRadians( 3 * factor), 0.2f * i++));
            mobile.UninitializedProjectileList.Add(new BigfootProjectile1(mobile, MathHelper.ToRadians(-3 * factor), 0.2f * i++));
            mobile.UninitializedProjectileList.Add(new BigfootProjectile1(mobile, MathHelper.ToRadians(0), 0.2f * i++));
            mobile.UninitializedProjectileList.Add(new BigfootProjectile1(mobile, MathHelper.ToRadians(0), 0.2f * i++));
        }

        public static void Shot2(Bigfoot mobile)
        {
            for (int j = 0; j < 2; j++)
                for (int i = 0; i < 3; i++)
                    mobile.UninitializedProjectileList.Add(new BigfootProjectile2(mobile, -4 + i * 4, 0.2f * (1 + j)));
        }

        public static void Shot3(Bigfoot mobile)
        {
            int factor = mobile.Facing == Facing.Left ? 1 : -1;

            for (int i = 0; i < 8;)
            {
                mobile.UninitializedProjectileList.Add(new BigfootProjectile3(mobile, MathHelper.ToRadians( 3 * factor), 0.125f * i++));
                mobile.UninitializedProjectileList.Add(new BigfootProjectile3(mobile, MathHelper.ToRadians(-3 * factor), 0.125f * i++));
                mobile.UninitializedProjectileList.Add(new BigfootProjectile3(mobile, MathHelper.ToRadians(0), 0.125f * i++));
                mobile.UninitializedProjectileList.Add(new BigfootProjectile3(mobile, MathHelper.ToRadians(0), 0.125f * i++));
            }
        }
    }

    public class BigfootProjectile1 : Projectile
    {
        public BigfootProjectile1(Bigfoot mobile, float angleModifier, float spawnTime)
            : base(mobile, ShotType.S1, Parameter.ProjectileBigfootS1ExplosionRadius, Parameter.ProjectileBigfootS1BaseDamage, angleModifier: angleModifier)
        {
            //Initializing Flipbook
            FlipbookList.Add(new Flipbook(
                mobile.Crosshair.CannonPosition, new Vector2(31.5f, 20),
                63, 40, "Graphics/Tank/Bigfoot/Bullet1",
                new List<AnimationInstance>() {
                    new AnimationInstance()
                    { StartingFrame = 0, EndingFrame = 4, TimePerFrame = 1 / 20f }
                }, DepthParameter.Projectile, angle));

            //Physics/Trajectory setups
            mass = Parameter.ProjectileBigfootS1Mass;
            windInfluence = Parameter.ProjectileBigfootS1WindInfluence;

            SpawnTime = spawnTime;
        }

        public override void Explode()
        {
            base.Explode();
            SpecialEffectBuilder.BigfootProjectile1Explosion(FlipbookList[0].Position);
        }

        protected override void Destroy()
        {
            base.Destroy();

            List<Projectile> pjList = Mobile.ProjectileList.Except(Mobile.UnusedProjectile).ToList();

            if (pjList.Count() == 0)
                OnFinalizeExecutionAction?.Invoke();
            else if (GameScene.Camera.TrackedObject == this)
                GameScene.Camera.TrackObject(pjList.First());
        }
    }

    public class BigfootProjectile2 : Projectile
    {
        public BigfootProjectile2(Bigfoot mobile, int forceModifier, float spawnTime)
            : base(mobile, ShotType.S2, Parameter.ProjectileBigfootS2ExplosionRadius, Parameter.ProjectileBigfootS2BaseDamage, forceModifier: forceModifier)
        {
            Mobile = mobile;

            //Initializing Flipbook
            FlipbookList.Add(new Flipbook(
                mobile.Crosshair.CannonPosition, new Vector2(9, 7.5f),
                18, 15, "Graphics/Tank/Bigfoot/Bullet2",
                new List<AnimationInstance>() {
                    new AnimationInstance()
                    { StartingFrame = 0, EndingFrame = 9, TimePerFrame = 1 / 20f }
                }, DepthParameter.Projectile, angle));

            //Physics/Trajectory setups
            mass = Parameter.ProjectileBigfootS2Mass;
            windInfluence = Parameter.ProjectileBigfootS2WindInfluence;

            SpawnTime = spawnTime;
        }

        public override void Explode()
        {
            SpecialEffectBuilder.ArmorProjectile2Explosion(FlipbookList[0].Position);
            base.Explode();
        }

        protected override void Destroy()
        {
            base.Destroy();

            List<Projectile> pjList = Mobile.ProjectileList.Except(Mobile.UnusedProjectile).ToList();

            if (pjList.Count() == 0)
                OnFinalizeExecutionAction?.Invoke();
            else if (GameScene.Camera.TrackedObject == this)
                GameScene.Camera.TrackObject(pjList.First());
        }
    }

    public class BigfootProjectile3 : Projectile
    {
        public BigfootProjectile3(Bigfoot mobile, float angleModifier, float spawnTime)
            : base(mobile, ShotType.SS, Parameter.ProjectileBigfootSSExplosionRadius, Parameter.ProjectileBigfootSSBaseDamage, angleModifier: angleModifier)
        {
            Mobile = mobile;

            //Initializing Flipbook
            FlipbookList.Add(new Flipbook(
                mobile.Crosshair.CannonPosition, new Vector2(34.5f, 17.5f),
                69, 35, "Graphics/Tank/Bigfoot/Bullet3",
                new List<AnimationInstance>() {
                    new AnimationInstance(){ StartingFrame = 0, EndingFrame = 4, TimePerFrame = 1 / 20f }
                }, DepthParameter.Projectile, angle));

            //Physics/Trajectory setups
            mass = Parameter.ProjectileBigfootSSMass;
            windInfluence = Parameter.ProjectileBigfootSSWindInfluence;

            SpawnTime = spawnTime;
        }

        public override void Explode()
        {
            SpecialEffectBuilder.BigfootProjectile3Explosion(FlipbookList[0].Position);
            base.Explode();
        }

        protected override void Destroy()
        {
            base.Destroy();

            List<Projectile> pjList = Mobile.ProjectileList.Except(Mobile.UnusedProjectile).ToList();

            if (pjList.Count() == 0)
                OnFinalizeExecutionAction?.Invoke();
            else if (GameScene.Camera.TrackedObject == this)
                GameScene.Camera.TrackObject(pjList.First());
        }
    }
}
