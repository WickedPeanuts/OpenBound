
using Microsoft.Xna.Framework;
using OpenBound.Common;
using OpenBound.Extension;
using OpenBound.GameComponents.Level.Scene;
using OpenBound_Network_Object_Library.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenBound.GameComponents.Animation
{
    public class ParticleBuilder
    {
        static readonly ParticleUpdateConfiguration groundParticleUpdateConfiguration = new ParticleUpdateConfiguration() { Rotate = true, SetScale = true, UseYAcceleration = true, UseXAcceleration = true };
        static readonly ParticleUpdateConfiguration forceParticleUpdateConfiguration = new ParticleUpdateConfiguration() { SetScale = true, Fade = true, Rotate = true, UseXAcceleration = true, UseYAcceleration = true };

        public static void AsyncCreateGroundCollapseParticleEffect(int numberOfParticles, Vector2 initialPosition, float angleTrajectory)
        {
            Thread t = new Thread(() => { CreateGroundCollapseParticleEffect(numberOfParticles, initialPosition, angleTrajectory); });
            t.Start();
        }

        public static void AsyncCreateForceCollapseParticleEffect(int numberOfParticles, Vector2 initialPosition, float angleTrajectory)
        {
            Thread t = new Thread(() => { CreateForceCollapseParticleEffect(numberOfParticles, initialPosition, angleTrajectory); });
            t.Start();
        }

        private static void CreateGroundCollapseParticleEffect(int numberOfParticles, Vector2 initialPosition, float angleTrajectory)
        {
            List<SpecialEffect> pList = new List<SpecialEffect>();
            List<Flipbook> flipbookList = BuildGroundParticle(numberOfParticles);
           
            float windAngle = LevelScene.MatchMetadata.WindAngleRadians;
            Vector2 windVector = new Vector2((float)Math.Cos(windAngle), (float)Math.Sin(windAngle)) * LevelScene.MatchMetadata.WindForce * Parameter.GroundParticleWindInfluenceFactor;

            for (int i = 0; i < flipbookList.Count; i++)
            {
                float spreadFactor = i % Parameter.GroundParticleSpreadFactor + 1;
                float disturbance = ((float)Parameter.Random.NextDouble() - 0.5f) * MathHelper.TwoPi / spreadFactor;
                float angularSpread = angleTrajectory + disturbance;

                flipbookList[i].Position = initialPosition;
                flipbookList[i].Rotation = angularSpread;

                pList.Add(new Particle(flipbookList[i], new Vector2((float)Math.Cos(angularSpread), (float)Math.Sin(angularSpread)), windVector, groundParticleUpdateConfiguration));
            }

            SpecialEffectHandler.AddRange(pList);
        }

        private static void CreateForceCollapseParticleEffect(int numberOfParticles, Vector2 initialPosition, float angleTrajectory)
        {
            List<SpecialEffect> pList = new List<SpecialEffect>();
            List<Flipbook> flipbookList = BuildForceParticle(numberOfParticles);

            for (int i = 0; i < flipbookList.Count; i++)
            {
                float angle = (float)Parameter.Random.NextDouble() * MathHelper.TwoPi;
                flipbookList[i].Position = initialPosition;
                flipbookList[i].Rotation = angle;

                pList.Add(new Particle(flipbookList[i], new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)), Vector2.Zero, forceParticleUpdateConfiguration));
            }

            SpecialEffectHandler.AddRange(pList);
        }

        private static List<Flipbook> BuildForceParticle(int particleNumber)
        {
            return BuildParticle(particleNumber, new Vector2(16, 16), "Graphics/Special Effects/Weather/ForceParticle", 8);
        }

        private static List<Flipbook> BuildGroundParticle(int particleNumber)
        {
            Map map = GameInformation.Instance.RoomMetadata.Map;
            return BuildParticle(particleNumber, map.GroundParticlePivot.ToVector2(), $"Graphics/Maps/{map.GameMap}/Particle", map.GroundParticleNumberOfFrames);
        }

        private static List<Flipbook> BuildParticle(int particleNumber, Vector2 pivot, string particlePath, int numberOfFrames)
        {
            List<Flipbook> flipbookList = new List<Flipbook>();

            for (; particleNumber > 0; particleNumber--)
            {
                AnimationInstance animationInstance = new AnimationInstance();
                animationInstance.StartingFrame = animationInstance.EndingFrame = Parameter.Random.Next(0, numberOfFrames);

                flipbookList.Add(new Flipbook(Vector2.Zero, pivot, (int)(pivot.X * 2), (int)(pivot.Y * 2),
                    particlePath, animationInstance, DepthParameter.Projectile));
            }

            return flipbookList;
        }
    }
}
