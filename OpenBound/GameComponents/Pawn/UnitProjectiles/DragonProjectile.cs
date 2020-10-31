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
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenBound.GameComponents.Pawn.UnitProjectiles
{
    public class DragonProjectileEmitter
    {
        public static void Shot2(Dragon mobile)
        {
            int factor = mobile.Facing == Facing.Left ? 1 : -1;
            int i = 0;
            mobile.UninitializedProjectileList.Add(new DragonProjectile2(mobile,  5, MathHelper.ToRadians(-3 * factor), 0.2f * i++));
            mobile.UninitializedProjectileList.Add(new DragonProjectile2(mobile,  0, MathHelper.ToRadians( 3 * factor), 0.2f * i++));
            mobile.UninitializedProjectileList.Add(new DragonProjectile2(mobile, 10, MathHelper.ToRadians(-3 * factor), 0.2f * i++));
            mobile.UninitializedProjectileList.Add(new DragonProjectile2(mobile,  0, MathHelper.ToRadians( 3 * factor), 0.2f * i++));

            GameScene.Camera.TrackObject(mobile.UninitializedProjectileList.First());
        }
    }

    public class DragonProjectile : Projectile
    {
        Vector2 previousSESpawnPosition;
        double currentDistance = 0;

        public DragonProjectile(Dragon mobile, int damage, int blastRadius, float forceModifier, float angleModifier, float spawnTime)
            : base(mobile, ShotType.S1, blastRadius, damage,
                  angleModifier: angleModifier,
                  forceModifier: forceModifier)
        {
            //Initializing Flipbook
            FlipbookList.Add(new Flipbook(
                mobile.Crosshair.CannonPosition, Vector2.One / 2,
                1, 1, "Misc/Dummy",
                new List<AnimationInstance>() {
                    new AnimationInstance()
                }, DepthParameter.Projectile, angle));

            SpawnTime = spawnTime;
            FlipbookList[0].HideElement();
            FlipbookList[0].Color = Color.Yellow;

            previousSESpawnPosition = FlipbookList[0].Position;
        }

        protected override void UpdatePosition()
        {
            base.UpdatePosition();

            Vector2 dist = (previousSESpawnPosition - FlipbookList[0].Position);
            currentDistance += Math.Abs(dist.X) + Math.Abs(dist.Y);

            if (currentDistance > Parameter.ProjectileParticleNewEmissionMaxDistance)
            {
                previousSESpawnPosition = FlipbookList[0].Position;
                currentDistance = 0;

                SpecialEffect se = SpecialEffectBuilder.DragonProjectile1Explosion(FlipbookList[0].Position, (float)Parameter.Random.NextDouble() * MathHelper.TwoPi, FlipbookList[0].LayerDepth);
                se.Flipbook.Scale = Vector2.One / 3;
                se.Flipbook.Color = FlipbookList[0].Color;
                se.Flipbook.CurrentAnimationInstance.TimePerFrame = 1 / 60f;
            }
        }

        public override void Explode()
        {
            base.Explode();
            SpecialEffectBuilder.DragonProjectile1Explosion(FlipbookList[0].Position, FlipbookList[0].Rotation);
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

    public class DragonProjectile1 : DragonProjectile
    {
        public DragonProjectile1(Dragon mobile)
            : base(mobile, Parameter.ProjectileDragonS1BaseDamage, Parameter.ProjectileDragonS1ExplosionRadius, 0, 0, 0.3f)
        {
            mass = Parameter.ProjectileDragonS1Mass;
            windInfluence = Parameter.ProjectileDragonS1WindInfluence;
        }
    }

    public class DragonProjectile2 : DragonProjectile
    {
        public DragonProjectile2(Dragon mobile, float forceModifier, float angleModifier, float spawnTime)
            : base(mobile, Parameter.ProjectileDragonS2BaseDamage, Parameter.ProjectileDragonS2ExplosionRadius, forceModifier, angleModifier, spawnTime)
        {
            mass = Parameter.ProjectileDragonS2Mass;
            windInfluence = Parameter.ProjectileDragonS2WindInfluence;
        }
    }

    public class DragonProjectile3 : Projectile
    {
        public DragonProjectile3(Dragon mobile) : base(mobile, ShotType.SS, 0, 0)
        {
            //Initializing Flipbook
            FlipbookList.Add(new Flipbook(
                mobile.Crosshair.CannonPosition, new Vector2(19.5f, 19.5f),
                39, 39, "Graphics/Tank/Dragon/Bullet3",
                new List<AnimationInstance>() {
                    new AnimationInstance(){ StartingFrame = 0, EndingFrame = 4, TimePerFrame = 1 / 20f }
                }, DepthParameter.Projectile, angle));

            //Physics/Trajectory setups
            mass = Parameter.ProjectileDragonSSMass;
            windInfluence = Parameter.ProjectileDragonSSWindInfluence;

            SpawnTime = 0.3f;
        }

        public override void Explode()
        {
            base.Explode();

            Vector2 offset = new Vector2(300, 300);

            for (int i = 0; i <= 4; i++)
            {
                float angle = MathHelper.ToRadians(20 * (i - 4 / 2)) - MathHelper.PiOver2;

                DragonProjectile3SS dp3s = new DragonProjectile3SS(
                    (Dragon)Mobile,
                    new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * offset + FlipbookList[0].Position,
                    FlipbookList[0].Position, 0.3f * i);

                dp3s.OnFinalizeExecutionAction = OnFinalizeExecutionAction;

                Mobile.LastCreatedProjectileList.Add(dp3s);
            }

            GameScene.Camera.TrackObject(Mobile.LastCreatedProjectileList.First());
        }

        protected override void OutofboundsDestroy()
        {
            OnFinalizeExecutionAction?.Invoke();
            Destroy();
        }
    }

    public class DragonProjectile3SS : Projectile
    {
        ProjectileAnimationState animationState;

        public DragonProjectile3SS(Dragon mobile, Vector2 initialPosition, Vector2 finalPosition, float spawnTime)
            : base(mobile, ShotType.SS, Parameter.ProjectileDragonSSExplosionRadius, Parameter.ProjectileDragonSSBaseDamage,
                  projectileInitialPosition: initialPosition)
        {
            //Calculate the angle of the swords
            double angle = (float)Helper.AngleBetween(finalPosition, initialPosition);

            //Initializing Flipbook
            FlipbookList.Add(new Flipbook(
                initialPosition, new Vector2(49.5f, 40),
                99, 80, "Graphics/Tank/Dragon/Shot3",
                new List<AnimationInstance>() {
                    new AnimationInstance() { StartingFrame =  0, EndingFrame = 17, TimePerFrame = 1/20f },
                    new AnimationInstance() { StartingFrame = 18, EndingFrame = 40, TimePerFrame = 1/20f },
                }, DepthParameter.Projectile, rotation: (float)angle));

            SpawnTime = spawnTime;

            animationState = ProjectileAnimationState.Spawning;

            IsAbleToRefreshPosition = false;

            InitializeMovement();
        }

        public override void InitializeMovement()
        {
            yMovement.Preset((float)Math.Sin(CurrentFlipbookRotation) * Parameter.ProjectileDragonSSESpeedStartingFactor, (float)Math.Sin(CurrentFlipbookRotation) * Parameter.ProjectileDragonSSEAccelerationStartingFactor);
            xMovement.Preset((float)Math.Cos(CurrentFlipbookRotation) * Parameter.ProjectileDragonSSESpeedStartingFactor, (float)Math.Cos(CurrentFlipbookRotation) * Parameter.ProjectileDragonSSEAccelerationStartingFactor);
        }

        protected override void UpdatePosition()
        {
            base.UpdatePosition();

            switch (animationState)
            {
                case ProjectileAnimationState.Spawning:
                    if (FlipbookList[0].FlipbookAnimationList.Count() == 1)
                    {
                        IsAbleToRefreshPosition = true;
                        animationState = ProjectileAnimationState.Moving;
                    }
                    break;
                case ProjectileAnimationState.Moving: break;
            }                
        }

        public override void Explode()
        {
            base.Explode();

            SpecialEffectBuilder.ArmorProjectile2Explosion(FlipbookList[0].Position);
            SpecialEffectBuilder.CommonFlameSS(FlipbookList[0].Position, FlipbookList[0].Rotation);
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
