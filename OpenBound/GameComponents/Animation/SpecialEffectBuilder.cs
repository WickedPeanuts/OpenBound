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
using OpenBound.GameComponents.Interface;
using OpenBound.GameComponents.Level;
using OpenBound.GameComponents.Level.Scene;
using OpenBound_Network_Object_Library.Entity;
using System;

namespace OpenBound.GameComponents.Animation
{
    public class SpecialEffectBuilder
    {
        #region Weather
        public static void ForceRandomParticle(Vector2 position)
        {
            int frame = Parameter.Random.Next(0, 8);

            SpecialEffect se = new SpecialEffect(new Flipbook(position, new Vector2(17, 17), 32, 32, "Graphics/Special Effects/Weather/ForceParticle",
                new AnimationInstance() { StartingFrame = frame, EndingFrame = frame }, DepthParameter.ProjectileSFX, (float)Parameter.Random.NextDouble() * MathHelper.TwoPi), 0);
            SpecialEffectHandler.Add(se);

            float transparency = 1;
            float transparencyFactor = 0.5f + (float)Parameter.Random.NextDouble();
            float scaleFactor = (float)Parameter.Random.NextDouble() - 0.5f;

            se.UpdateAction += (a, b) =>
            {
                se.Flipbook.SetTransparency(Math.Max(0, transparency -= transparencyFactor * (float)b.ElapsedGameTime.TotalSeconds));
                se.Flipbook.Scale += Vector2.One * scaleFactor * (float)b.ElapsedGameTime.TotalSeconds;

                if (transparency <= 0)
                    SpecialEffectHandler.Remove(se);
            };
        }

        public static SpecialEffect ElectricityParticle(Vector2 position)
        {
            SpecialEffect se = new SpecialEffect(new Flipbook(position, new Vector2(16, 16), 32, 32, "Graphics/Special Effects/Weather/ElectricityParticle",
                new AnimationInstance() { EndingFrame = 8, TimePerFrame = 1 / 15f, AnimationType = AnimationType.Cycle }, DepthParameter.ProjectileSFX, (float)Parameter.Random.NextDouble() * MathHelper.TwoPi), 0);
            se.Flipbook.JumpToRandomAnimationFrame();
            SpecialEffectHandler.Add(se);
            return se;
        }
        #endregion
        #region Thor
        private static void ThorShotColorBase(Vector2 position, Color baseColor, float scale, float rotation)
        {
            Flipbook fb = new Flipbook(position, new Vector2(8, 128), 16, 256, "Graphics/Entity/Thor/ThorLaser", new AnimationInstance(), DepthParameter.ProjectileSFXBase, rotation);
            fb.Scale = new Vector2(3, scale);

            SpecialEffect se = new SpecialEffect(fb, 1f);

            float elapsedTime = 1f;
            se.UpdateAction += (specialEffect, gameTime) =>
            {
                elapsedTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                se.Flipbook.Color = baseColor * elapsedTime;
            };

            SpecialEffectHandler.Add(se);
        }

        private static void ThorShotBase(Vector2 position, float scale, float rotation)
        {
            Flipbook fb = new Flipbook(position, new Vector2(8, 128), 16, 256, "Graphics/Entity/Thor/ThorLaser", new AnimationInstance(), DepthParameter.ProjectileSFX, rotation);
            Vector2 originalScale = new Vector2(3, scale);
            fb.Scale = originalScale;

            SpecialEffect se = new SpecialEffect(fb, 1f);

            float transparency = 1f;
            se.UpdateAction += (specialEffect, gameTime) =>
            {
                transparency -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                se.Flipbook.SetTransparency(transparency * 2);
                se.Flipbook.Scale = new Vector2(se.Flipbook.Scale.X * transparency, originalScale.Y - ((originalScale.Y * (1 - transparency)) / (originalScale.Y * 2)));
            };

            SpecialEffectHandler.Add(se);
        }

        public static void ThorShot(Vector2 position, Color baseColor, float scale, float rotation)
        {
            ThorShotColorBase(position, baseColor, scale, rotation);
            ThorShotBase(position, scale, rotation);
        }
        #endregion

        #region Common
        public static void BlueSphereHitEffect(Vector2 position) =>
            HitEffect(position, 4, (float)Parameter.Random.NextDouble() * MathHelper.TwoPi);

        public static void HitEffect(Vector2 position, int frameIndex, float rotation)
        {
            Flipbook fb = new Flipbook(position, new Vector2(16, 16), 32, 32, "Graphics/Special Effects/General/Hit",
                new AnimationInstance() { StartingFrame = frameIndex, EndingFrame = frameIndex },
                DepthParameter.ProjectileSFX, rotation + MathHelper.PiOver2);

            SpecialEffect se = new SpecialEffect(fb, 1f);

            float scale = 0;

            se.UpdateAction += (specialEffect, gameTime) =>
            {
                scale += (float)gameTime.ElapsedGameTime.TotalSeconds * 4;
                float tScale = (float)Math.Sin(scale);

                se.Flipbook.Scale = new Vector2(0.2f + tScale, tScale * 18);
                se.Flipbook.SetTransparency(1 - scale);
            };

            SpecialEffectHandler.Add(se);
        }

        public static void TeleportFlame1(Vector2 position, float scale = 1)
        {
            SpecialEffectHandler.Add(
                new SpecialEffect(
                    new Flipbook(position, new Vector2(95, 100), 190, 192, "Graphics/Special Effects/General/Teleport1",
                        new AnimationInstance() { StartingFrame = 0, EndingFrame = 18, TimePerFrame = 1 / 30f },
                        DepthParameter.ProjectileSFX, (float)Parameter.Random.NextDouble() * MathHelper.TwoPi)
                    {
                        Scale = Vector2.One * scale
                    },
                    1));
        }

        public static void TeleportFlame2(Vector2 position, float scale = 1)
        {
            SpecialEffectHandler.Add(new SpecialEffect(new Flipbook(position, new Vector2(95, 100), 190, 192, "Graphics/Special Effects/General/Teleport2",
                new AnimationInstance() { StartingFrame = 0, EndingFrame = 18, TimePerFrame = 1 / 30f },
                DepthParameter.ProjectileSFX, (float)Parameter.Random.NextDouble() * MathHelper.TwoPi)
            {
                Scale = Vector2.One * scale
            }, 1));
        }

        public static void CommonFlameSS(Vector2 position, float rotation)
        {
            SpecialEffectHandler.Add(new SpecialEffect(new Flipbook(position, new Vector2(185, 170) / 2, 185, 170, "Graphics/Special Effects/Tank/Common/FlameSS",
                new AnimationInstance() { StartingFrame = 0, EndingFrame = 30, TimePerFrame = 1 / 30f }, DepthParameter.ProjectileSFX, rotation - MathHelper.PiOver2), 1));
        }

        public static void RandomLowHealthSmokeParticle(Vector2 position)
        {
            if (Parameter.Random.NextDouble() < 0.5)
                LowHealthSmokeParticle1(position);
            else
                LowHealthSmokeParticle2(position);
        }

        public static void LowHealthSmokeParticle1(Vector2 position) =>
            LowHealthSmokeParticle(position, "Graphics/Special Effects/General/Smoke1");

        public static void LowHealthSmokeParticle2(Vector2 position) =>
            LowHealthSmokeParticle(position, "Graphics/Special Effects/General/Smoke2");

        private static void LowHealthSmokeParticle(Vector2 position, string smokePath)
        {
            Flipbook fb = new Flipbook(position, new Vector2(16, 16), 32, 32, smokePath,
                new AnimationInstance() { StartingFrame = 0, EndingFrame = 7, TimePerFrame = 1 / 15f },
                DepthParameter.ProjectileSFX, (float)Parameter.Random.NextDouble() * MathHelper.TwoPi);

            float density = (float)(1/3f + 2 * Parameter.Random.NextDouble() / 3);

            fb.SetTransparency(density);
            fb.Scale *= density;

            SpecialEffect se = new SpecialEffect(fb, 1);
            
            MatchMetadata mm = LevelScene.MatchMetadata;

            Vector2 posFactor = 
                -Vector2.UnitY * 2 +
                Vector2.Transform(
                    Vector2.UnitX, 
                    Matrix.CreateRotationZ(
                        mm.WindAngleRadians + 
                        MathHelper.ToRadians(
                            (0.5f - (float)Parameter.Random.NextDouble()) * 30)) *
                            mm.WindForce / 5);

            if (Math.Abs(posFactor.Y) < 1)
                posFactor = new Vector2(posFactor.X, (posFactor.Y < 0) ? -1 : 1);

            float rotationFactor = (mm.WindForce / 10) * (0.5f - (float)Parameter.Random.NextDouble());
            float scaleFactor = (float)Parameter.Random.NextDouble();

            se.UpdateAction += (specialEffect, gameTime) =>
            {
                float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

                se.Flipbook.Position += posFactor;
                se.Flipbook.Rotation += rotationFactor;

                se.Flipbook.Scale *= 1 + scaleFactor * elapsedTime * 2;
            };

            SpecialEffectHandler.Add(se);
        }
        #endregion

        #region Armor
        public static void ArmorProjectile1Explosion(Vector2 position)
        {
            SpecialEffectHandler.Add(new SpecialEffect(new Flipbook(position, new Vector2(96, 96), 192, 192, "Graphics/Special Effects/Tank/Armor/Flame1",
                new AnimationInstance() { StartingFrame = 0, EndingFrame = 19, TimePerFrame = 1 / 30f }, DepthParameter.Projectile), 1));
        }

        public static void ArmorProjectile2Explosion(Vector2 position)
        {
            SpecialEffectHandler.Add(new SpecialEffect(new Flipbook(position, new Vector2(87, 91), 175, 183, "Graphics/Special Effects/Tank/Armor/Flame2",
                new AnimationInstance() { StartingFrame = 0, EndingFrame = 19, TimePerFrame = 1 / 30f }, DepthParameter.Projectile), 1));
        }

        public static void ArmorProjectile3Explosion(Vector2 position)
        {
            SpecialEffectHandler.Add(new SpecialEffect(new Flipbook(position, new Vector2(96, 96), 192, 192, "Graphics/Special Effects/Tank/Armor/Flame3",
                new AnimationInstance() { StartingFrame = 0, EndingFrame = 29, TimePerFrame = 1 / 30f }, DepthParameter.Projectile), 1));
        }
        #endregion
        #region Bigfoot
        public static void BigfootProjectile1Explosion(Vector2 position)
        {
            SpecialEffectHandler.Add(new SpecialEffect(new Flipbook(position, new Vector2(166, 158) / 2, 166, 158, "Graphics/Special Effects/Tank/Bigfoot/Flame1",
                new AnimationInstance() { StartingFrame = 0, EndingFrame = 19, TimePerFrame = 1 / 30f }, DepthParameter.ProjectileSFX), 1));
        }

        public static void BigfootProjectile2Explosion(Vector2 position)
        {
            SpecialEffectHandler.Add(new SpecialEffect(new Flipbook(position, new Vector2(78, 73) / 2, 78, 73, "Graphics/Special Effects/Tank/Bigfoot/Flame2",
                new AnimationInstance() { StartingFrame = 0, EndingFrame = 19, TimePerFrame = 1 / 30f }, DepthParameter.ProjectileSFX), 1)); ;
        }

        public static void BigfootProjectile3Explosion(Vector2 position)
        {
            SpecialEffectHandler.Add(new SpecialEffect(new Flipbook(position, new Vector2(96, 96), 192, 192, "Graphics/Special Effects/Tank/Bigfoot/Flame3",
                new AnimationInstance() { StartingFrame = 0, EndingFrame = 29, TimePerFrame = 1 / 30f }, DepthParameter.ProjectileSFX), 1));
        }
        #endregion
        #region Dragon
        public static SpecialEffect DragonProjectile1Explosion(Vector2 position, float rotation, float layerDepth = DepthParameter.ProjectileSFX)
        {
            Flipbook fb = new Flipbook(position, new Vector2(94, 92.5f), 188, 185, "Graphics/Special Effects/Tank/Dragon/Flame1",
                new AnimationInstance() { StartingFrame = 0, EndingFrame = 30, TimePerFrame = 1 / 30f }, layerDepth, rotation);

            SpecialEffect se = new SpecialEffect(fb, 1);

            SpecialEffectHandler.Add(se);

            return se;
        }
        #endregion
        #region Ice
        public static void IceProjectile1Explosion(Vector2 position, float rotation)
        {
            Flipbook fb = new Flipbook(position, new Vector2(90, 86), 180, 172, "Graphics/Special Effects/Tank/Ice/Flame1",
                new AnimationInstance() { StartingFrame = 0, EndingFrame = 16, TimePerFrame = 1 / 30f }, DepthParameter.ProjectileSFX, rotation);

            SpecialEffect se = new SpecialEffect(fb, 1);

            SpecialEffectHandler.Add(se);
        }

        public static void IceProjectile2Explosion(Vector2 position, float rotation)
        {
            Flipbook fb = new Flipbook(position, new Vector2(87.5f, 87f), 175, 174, "Graphics/Special Effects/Tank/Ice/Flame2",
                new AnimationInstance() { StartingFrame = 0, EndingFrame = 27, TimePerFrame = 1 / 30f }, DepthParameter.ProjectileSFX, rotation);

            SpecialEffect se = new SpecialEffect(fb, 1);
            SpecialEffectHandler.Add(se);
        }

        public static void IceProjectile3Explosion(Vector2 position)
        {
            Flipbook fb = new Flipbook(position, new Vector2(185, 187), 370, 374, "Graphics/Special Effects/Tank/Ice/Flame3",
                new AnimationInstance() { StartingFrame = 0, EndingFrame = 30, TimePerFrame = 1 / 30f }, DepthParameter.ProjectileSFX);

            SpecialEffect se = new SpecialEffect(fb, 1);

            SpecialEffectHandler.Add(se);
        }
        #endregion
        #region Knight
        public static SpecialEffect KnightProjectileBullet1(Vector2 position, float rotation)
        {
            SpecialEffect se = new SpecialEffect(new Flipbook(position, new Vector2(81, 32), 162, 65, "Graphics/Tank/Knight/Bullet1",
                new AnimationInstance() { StartingFrame = 0, EndingFrame = 19, TimePerFrame = 1 / 30f }, DepthParameter.ProjectileSFX, rotation), 0);
            SpecialEffectHandler.Add(se);
            return se;
        }
        #endregion
        #region Mage
        public static void MageProjectile1Explosion(Vector2 position, float rotation)
        {
            Flipbook fb = new Flipbook(position, new Vector2(96.5f, 96), 193, 192, "Graphics/Special Effects/Tank/Mage/Flame1",
                new AnimationInstance() { StartingFrame = 0, EndingFrame = 17, TimePerFrame = 1 / 30f }, DepthParameter.ProjectileSFX, rotation);

            SpecialEffect se = new SpecialEffect(fb, 1);

            SpecialEffectHandler.Add(se);
        }

        public static void MageProjectile2Explosion(Vector2 position, float rotation)
        {
            Flipbook fb = new Flipbook(position, new Vector2(82, 82), 143, 142, "Graphics/Special Effects/Tank/Mage/Flame2",
                new AnimationInstance() { StartingFrame = 0, EndingFrame = 17, TimePerFrame = 1 / 30f }, DepthParameter.ProjectileSFX, rotation);

            SpecialEffect se = new SpecialEffect(fb, 1);

            SpecialEffectHandler.Add(se);
        }

        public static void MageProjectile3Explosion(Vector2 position, float rotation)
        {
            Flipbook fb = new Flipbook(position, new Vector2(192.5f, 192), 385, 384, "Graphics/Special Effects/Tank/Mage/Flame3",
                new AnimationInstance() { StartingFrame = 0, EndingFrame = 30, TimePerFrame = 1 / 30f }, DepthParameter.ProjectileSFX, rotation);

            SpecialEffect se = new SpecialEffect(fb, 1);

            float transparency = 1f;
            se.UpdateAction += (specialEffect, gameTime) =>
            {
                se.Flipbook.Rotation += MathHelper.Pi * (float)gameTime.ElapsedGameTime.TotalSeconds;
                transparency -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                se.Flipbook.SetTransparency(transparency);
            };

            SpecialEffectHandler.Add(se);
        }
        #endregion
        #region RaonLauncher
        public static SpecialEffect RaonLauncherProjectile1(Vector2 position, float rotation)
        {
            Flipbook fb = new Flipbook(
                position, new Vector2(19, 21),
                38, 42, "Graphics/Tank/RaonLauncher/Bullet1",
                new AnimationInstance() { StartingFrame = 0, EndingFrame = 39, TimePerFrame = 0f },
                DepthParameter.Projectile, rotation);

            SpecialEffect se = new SpecialEffect(fb, 0);

            return se;
        }

        
        public static void RaonLauncherProjectile1Explosion(Vector2 position)
        {
            Flipbook fb = new Flipbook(position, new Vector2(94, 92), 169, 164, "Graphics/Special Effects/Tank/RaonLauncher/Flame1",
                new AnimationInstance() { StartingFrame = 0, EndingFrame = 19, TimePerFrame = 1 / 30f }, DepthParameter.ProjectileSFX, (float)Parameter.Random.NextDouble() * MathHelper.TwoPi);

            SpecialEffect se = new SpecialEffect(fb, 1);

            SpecialEffectHandler.Add(se);
        }

        public static void RaonLauncherProjectile2Explosion(Vector2 position)
        {
            Flipbook fb = new Flipbook(position, new Vector2(96, 96), 157, 167, "Graphics/Special Effects/Tank/RaonLauncher/Flame2",
                new AnimationInstance() { StartingFrame = 0, EndingFrame = 19, TimePerFrame = 1 / 30f }, DepthParameter.ProjectileSFX, (float)Parameter.Random.NextDouble() * MathHelper.TwoPi);

            SpecialEffect se = new SpecialEffect(fb, 1);

            SpecialEffectHandler.Add(se);
        }

        public static void RaonLauncherProjectile2DormantTornado(Vector2 position)
        {
            Flipbook fb = new Flipbook(position, new Vector2(15.5f, 18.5f), 31, 29, "Graphics/Tank/RaonLauncherMineS2/CharacterSpritesheet",
                new AnimationInstance() { StartingFrame = 0, EndingFrame = 19, TimePerFrame = 1 / 10f }, DepthParameter.ProjectileSFX, 0);

            SpecialEffect se = new SpecialEffect(fb, 0);

            float positionMultiplier = 0;
            se.UpdateAction += (specialEffect, gameTime) =>
            {
                if (Topography.IsNotInsideMapBoundaries(se.Flipbook.Position))
                    SpecialEffectHandler.Remove(se);

                se.Flipbook.Position -= new Vector2(0, positionMultiplier);
                se.Flipbook.Rotation += (float)gameTime.ElapsedGameTime.TotalSeconds * positionMultiplier * positionMultiplier * MathHelper.PiOver2;

                positionMultiplier += (float)gameTime.ElapsedGameTime.TotalSeconds;
            };

            SpecialEffectHandler.Add(se);
        }

        public static void RaonLauncherProjectile2ActiveTornado(Vector2 position)
        {
            Flipbook fb = new Flipbook(position, new Vector2(15.5f, 18.5f), 31, 29, "Graphics/Tank/RaonLauncherMineS2/CharacterSpritesheet",
                new AnimationInstance() { StartingFrame = 20, EndingFrame = 36, TimePerFrame = 1 / 10f }, DepthParameter.ProjectileSFX, 0);

            SpecialEffect se = new SpecialEffect(fb, 0);

            float positionMultiplier = 0;
            se.UpdateAction += (specialEffect, gameTime) =>
            {
                if (Topography.IsNotInsideMapBoundaries(se.Flipbook.Position))
                    SpecialEffectHandler.Remove(se);

                se.Flipbook.Position -= new Vector2(0, positionMultiplier);
                se.Flipbook.Rotation += (float)gameTime.ElapsedGameTime.TotalSeconds * positionMultiplier * positionMultiplier * MathHelper.PiOver2;

                positionMultiplier += (float)gameTime.ElapsedGameTime.TotalSeconds;
            };

            SpecialEffectHandler.Add(se);
        }

        public static void RaonLauncherProjectile3Explosion(Vector2 position)
        {
            Flipbook fb = new Flipbook(position, new Vector2(94, 95), 172, 182, "Graphics/Special Effects/Tank/RaonLauncher/Flame3",
                new AnimationInstance() { StartingFrame = 0, EndingFrame = 29, TimePerFrame = 1 / 30f }, DepthParameter.ProjectileSFX, (float)Parameter.Random.NextDouble() * MathHelper.TwoPi);

            SpecialEffect se = new SpecialEffect(fb, 1);

            SpecialEffectHandler.Add(se);
        }
        #endregion
        #region Turtle
        public static void TurtleProjectile1Explosion(Vector2 position, float rotation)
        {
            Flipbook fb = new Flipbook(position, new Vector2(104f, 105f), 193, 200, "Graphics/Special Effects/Tank/Turtle/Flame1",
                new AnimationInstance() { StartingFrame = 0, EndingFrame = 18, TimePerFrame = 1 / 30f }, DepthParameter.ProjectileSFX, rotation);

            SpecialEffect se = new SpecialEffect(fb, 1);

            SpecialEffectHandler.Add(se);
        }

        public static void TurtleProjectile2Explosion(Vector2 position, float rotation)
        {
            Flipbook fb = new Flipbook(position, new Vector2(86f, 89f), 164, 170, "Graphics/Special Effects/Tank/Turtle/Flame2",
                new AnimationInstance() { StartingFrame = 0, EndingFrame = 18, TimePerFrame = 1 / 30f }, DepthParameter.ProjectileSFX, rotation);

            SpecialEffect se = new SpecialEffect(fb, 1);

            SpecialEffectHandler.Add(se);
        }

        public static void TurtleProjectile3Explosion(Vector2 position, float rotation)
        {
            Flipbook fb = new Flipbook(position, new Vector2(90f, 93f), 159, 173, "Graphics/Special Effects/Tank/Turtle/Flame3",
                new AnimationInstance() { StartingFrame = 0, EndingFrame = 28, TimePerFrame = 1 / 30f }, DepthParameter.ProjectileSFX, rotation);

            SpecialEffect se = new SpecialEffect(fb, 1);

            SpecialEffectHandler.Add(se);
        }

        public static void TurtleProjectile3EExplosion(Vector2 position, float rotation)
        {
            Flipbook fb = new Flipbook(position, new Vector2(92f, 95f), 157, 188, "Graphics/Special Effects/Tank/Turtle/Flame3E",
                new AnimationInstance() { StartingFrame = 0, EndingFrame = 15, TimePerFrame = 1 / 30f }, DepthParameter.ProjectileSFX, rotation);

            SpecialEffect se = new SpecialEffect(fb, 1);

            SpecialEffectHandler.Add(se);
        }

        public static void TurtleProjectile3Division(Vector2 position, float rotation)
        {
            Flipbook fb = new Flipbook(position, new Vector2(78f, 65f), 186, 129, "Graphics/Special Effects/Tank/Turtle/Flame3Divide",
                new AnimationInstance() { StartingFrame = 0, EndingFrame = 10, TimePerFrame = 1 / 30f }, DepthParameter.ProjectileSFX, rotation);

            SpecialEffect se = new SpecialEffect(fb, 1);

            SpecialEffectHandler.Add(se);
        }
        #endregion
        #region Trico
        public static void TricoProjectile1Explosion(Vector2 position, float rotation)
        {
            Flipbook fb = new Flipbook(position, new Vector2(90f, 100f), 192, 192, "Graphics/Special Effects/Tank/Trico/Flame1",
                new AnimationInstance() { StartingFrame = 0, EndingFrame = 19, TimePerFrame = 1 / 30f }, DepthParameter.ProjectileSFX, rotation);
            SpecialEffect se = new SpecialEffect(fb, 1);
            SpecialEffectHandler.Add(se);
        }

        public static void TricoProjectile2Explosion(Vector2 position, float rotation)
        {
            Flipbook fb = new Flipbook(position, new Vector2(90f, 100f), 192, 192, "Graphics/Special Effects/Tank/Trico/Flame1",
                new AnimationInstance() { StartingFrame = 0, EndingFrame = 19, TimePerFrame = 1 / 30f }, DepthParameter.ProjectileSFX, rotation);
            fb.Scale = new Vector2(0.7f, 0.7f);
            fb.Effect = SpriteEffects.FlipVertically;
            SpecialEffect se = new SpecialEffect(fb, 1);
            SpecialEffectHandler.Add(se);
        }

        public static void TricoProjectile3Explosion(Vector2 position)
        {
            Flipbook fb = new Flipbook(position, new Vector2(179f, 197f), 311, 313, "Graphics/Special Effects/Tank/Trico/Flame3",
                new AnimationInstance() { StartingFrame = 0, EndingFrame = 29, TimePerFrame = 1 / 20f }, DepthParameter.ProjectileSFX, 0);
            SpecialEffect se = new SpecialEffect(fb, 1);

            SpecialEffectHandler.Add(se);
        }
        #endregion
        #region Lightning
        public static void LightningProjectile3Explosion(Vector2 position, float rotation)
        {
            Flipbook fb = new Flipbook(position, new Vector2(196, 193), 324, 326, "Graphics/Special Effects/Tank/Lightning/Flame3",
                new AnimationInstance() { StartingFrame = 0, EndingFrame = 20, TimePerFrame = 1 / 20f }, DepthParameter.ProjectileSFX, rotation + MathHelper.PiOver2);

            SpecialEffect se = new SpecialEffect(fb, 1);

           // float transparency = 1f;
            se.UpdateAction += (specialEffect, gameTime) =>
            {
                se.Flipbook.Rotation -= MathHelper.TwoPi * (float)gameTime.ElapsedGameTime.TotalSeconds / 25;
                //transparency -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                //se.Flipbook.SetTransparency(transparency);
            };

            SpecialEffectHandler.Add(se);
        }
        #endregion

    }
}
